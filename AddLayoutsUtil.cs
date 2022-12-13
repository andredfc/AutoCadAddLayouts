using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System.IO;
using Autodesk.AutoCAD.Geometry;

namespace AutoCadAddLayouts
{
    public class AddLayoutsUtil

    {

        [CommandMethod("ADDLAYOUTS")]
        public void AddLayoutsStart()
        {
            AddLayoutsForm AddLayoutsForm = new AddLayoutsForm();
            AddLayoutsForm.Show();
        }


        public void AddLayouts(IEnumerable<string> filePaths)
        {
            Document currentDoc = Application.DocumentManager.MdiActiveDocument;

            List<BlockTableRecord> blocks = new List<BlockTableRecord>();

            foreach (var filePath in filePaths)
            {
                string fileName = Path.GetFileName(filePath).Replace(".dwg", "");

                Document dwgDoc = Application.DocumentManager.Open(filePath);
                Database dwgDB = dwgDoc.Database;

                dwgDoc.LockDocument();

                using (Transaction dwgTrans = dwgDB.TransactionManager.StartTransaction())
                {
                    Application.DocumentManager.MdiActiveDocument = dwgDoc;
                    var layoutId = LayoutManager.Current.GetLayoutId("Model");

                    var layout = (Layout)dwgTrans.GetObject(layoutId, OpenMode.ForRead, false);

                    List<Point3d> minPoints = new List<Point3d>();
                    List<Point3d> maxPoints = new List<Point3d>();

                    using (BlockTableRecord ms = dwgTrans.GetObject(layout.BlockTableRecordId, OpenMode.ForRead) as BlockTableRecord)
                    {
                        foreach (ObjectId id in ms)
                        {
                            using (Entity ent = dwgTrans.GetObject(id, OpenMode.ForWrite) as Entity)
                            {
                                var ext = ent.Bounds;
                                if (ext != null)
                                {
                                    minPoints.Add(ext.Value.MinPoint);
                                    maxPoints.Add(ext.Value.MaxPoint);
                                }
                            }

                        }
                    }
                    Point3d minPoint = new Point3d(minPoints.Min(x => x.X), minPoints.Min(x => x.Y), minPoints.Min(x => x.Z));
                    Point3d maxPoint = new Point3d(maxPoints.Max(x => x.X), maxPoints.Max(x => x.Y), maxPoints.Max(x => x.Z));

                    CopyLayout(currentDoc, fileName, layout.BlockTableRecordId, dwgDB, minPoint, maxPoint, dwgTrans);

                    dwgTrans.Commit();
                }
                dwgDoc.CloseAndDiscard();
            }
        }

        private void CopyLayout(Document destDoc, string fileName, ObjectId blocktableRecordId, Database sourceDb, Point3d minPoint, Point3d maxPoint, Transaction dwgTrans)
        {
            Application.DocumentManager.MdiActiveDocument = destDoc;
            Database currentDB = destDoc.Database;


            using (destDoc.LockDocument())
            {
                using (Transaction currTrans = currentDB.TransactionManager.StartTransaction())
                {
                    var newLayoutId = LayoutManager.Current.CreateAndMakeLayoutCurrent(fileName, true);

                    var lay = (Layout)currTrans.GetObject(newLayoutId, OpenMode.ForWrite);

                    var sourceId = blocktableRecordId;

                    ObjectId sourceMsId = sourceId;
                    ObjectId destDbMsId = SymbolUtilityServices.GetBlockPaperSpaceId(currentDB);

                    ObjectIdCollection sourceIds = GetObjectIdCollection(sourceMsId, dwgTrans);

                    IdMapping mapping = new IdMapping();

                    sourceDb.WblockCloneObjects(sourceIds, destDbMsId, mapping, DuplicateRecordCloning.Replace, false);

                    var ext = lay.GetMaximumExtents();

                    var difObjX = maxPoint.X - minPoint.X;
                    var difObjY = maxPoint.Y - minPoint.Y;
                    var difLayX = ext.MaxPoint.X - ext.MinPoint.X;
                    var difLayY = ext.MaxPoint.Y - ext.MinPoint.Y;

                    double difObj = difObjX > difObjY ? difObjX : difObjY;
                    double difLay = difLayX < difLayY ? difLayX : difLayY;
                    double scale = difObj == 0 ? 1 : (difLay / difObj) * 0.9;

                    using (BlockTableRecord ms = currTrans.GetObject(destDbMsId, OpenMode.ForRead) as BlockTableRecord)
                    {
                        foreach (ObjectId id in ms)
                        {
                            using (Entity ent = currTrans.GetObject(id, OpenMode.ForWrite) as Entity)
                            {

                                Matrix3d mat = Matrix3d.Scaling(scale, new Point3d(-minPoint.X * scale + difLayX * 0.5 - difObjX * scale * 0.5, -minPoint.Y * scale + difLayY * 0.5 - difObjY * scale * 0.5, 0));
                                ent.TransformBy(mat);

                            }
                        }
                    }

                    lay.GetViewports();

                    lay.SetPlotSettings("ANSI_B_(11.00_x_17.00_Inches)", "monochrome.ctb", "DWF6 ePlot.pc3");

                    lay.ApplyToViewport(

                      currTrans, 2,

                      vp =>
                      {
                          vp.ResizeViewport(ext, 0.9);
                          if (ValidDbExtents(currentDB.Extmin, currentDB.Extmax))
                          {
                              vp.FitContentToViewport(new Extents3d(currentDB.Extmin, currentDB.Extmax), 0.9);
                          }
                          vp.Locked = true;
                      }
                    );
                    currTrans.Commit();
                }
            }
        }

        private ObjectIdCollection GetObjectIdCollection(ObjectId sourceMsId, Transaction tr)
        {
            ObjectIdCollection sourceIds = new ObjectIdCollection();
            using (BlockTableRecord ms = tr.GetObject(sourceMsId, OpenMode.ForRead) as BlockTableRecord)
            {
                foreach (ObjectId id in ms)
                {
                    sourceIds.Add(id);
                }
            }
            return sourceIds;
        }

        private bool ValidDbExtents(Point3d min, Point3d max)
        {
            return

              !(min.X > 0 && min.Y > 0 && min.Z > 0 &&
                max.X < 0 && max.Y < 0 && max.Z < 0);

        }
    }

    public static class Extensions

    {

        /// <summary>

        /// Reverses the order of the X and Y properties of a Point2d.

        /// </summary>

        /// <param name="flip">Boolean indicating whether to reverse or not.</param>

        /// <returns>The original Point2d or the reversed version.</returns>



        public static Point2d Swap(this Point2d pt, bool flip = true)

        {

            return flip ? new Point2d(pt.Y, pt.X) : pt;

        }



        /// <summary>

        /// Pads a Point2d with a zero Z value, returning a Point3d.

        /// </summary>

        /// <param name="pt">The Point2d to pad.</param>

        /// <returns>The padded Point3d.</returns>



        public static Point3d Pad(this Point2d pt)

        {

            return new Point3d(pt.X, pt.Y, 0);

        }



        /// <summary>

        /// Strips a Point3d down to a Point2d by simply ignoring the Z ordinate.

        /// </summary>

        /// <param name="pt">The Point3d to strip.</param>

        /// <returns>The stripped Point2d.</returns>



        public static Point2d Strip(this Point3d pt)

        {

            return new Point2d(pt.X, pt.Y);

        }



        /// <summary>

        /// Creates a layout with the specified name and optionally makes it current.

        /// </summary>

        /// <param name="name">The name of the viewport.</param>

        /// <param name="select">Whether to select it.</param>

        /// <returns>The ObjectId of the newly created viewport.</returns>



        public static ObjectId CreateAndMakeLayoutCurrent(

          this LayoutManager lm, string name, bool select = true

        )

        {

            // First try to get the layout



            var id = lm.GetLayoutId(name);



            // If it doesn't exist, we create it



            if (!id.IsValid)

            {

                id = lm.CreateLayout(name);

            }

            // And finally we select it

            if (select)

            {

                lm.CurrentLayout = name;

            }

            return id;
        }
        /// <summary>

        /// Applies an action to the specified viewport from this layout.

        /// Creates a new viewport if none is found withthat number.

        /// </summary>

        /// <param name="tr">The transaction to use to open the viewports.</param>

        /// <param name="vpNum">The number of the target viewport.</param>

        /// <param name="f">The action to apply to each of the viewports.</param>

        public static void ApplyToViewport(

          this Layout lay, Transaction tr, int vpNum, Action<Viewport> f

        )

        {

            var vpIds = lay.GetViewports();

            Viewport vp = null;



            foreach (ObjectId vpId in vpIds)

            {

                var vp2 = tr.GetObject(vpId, OpenMode.ForWrite) as Viewport;

                if (vp2 != null && vp2.Number == vpNum)

                {

                    // We have found our viewport, so call the action



                    vp = vp2;

                    break;

                }

            }



            if (vp == null)

            {

                // We have not found our viewport, so create one



                var btr =

                  (BlockTableRecord)tr.GetObject(

                    lay.BlockTableRecordId, OpenMode.ForWrite

                  );



                vp = new Viewport();



                // Add it to the database



                btr.AppendEntity(vp);

                tr.AddNewlyCreatedDBObject(vp, true);



                // Turn it - and its grid - on



                vp.On = true;

                vp.GridOn = true;

            }



            // Finally we call our function on it



            f(vp);

        }



        /// <summary>

        /// Apply plot settings to the provided layout.

        /// </summary>

        /// <param name="pageSize">The canonical media name for our page size.</param>

        /// <param name="styleSheet">The pen settings file (ctb or stb).</param>

        /// <param name="devices">The name of the output device.</param>



        public static void SetPlotSettings(

          this Layout lay, string pageSize, string styleSheet, string device

        )

        {

            using (var ps = new PlotSettings(lay.ModelType))

            {

                ps.CopyFrom(lay);



                var psv = PlotSettingsValidator.Current;



                // Set the device



                var devs = psv.GetPlotDeviceList();

                if (devs.Contains(device))

                {

                    psv.SetPlotConfigurationName(ps, device, null);

                    psv.RefreshLists(ps);

                }



                // Set the media name/size



                var mns = psv.GetCanonicalMediaNameList(ps);

                if (mns.Contains(pageSize))

                {

                    psv.SetCanonicalMediaName(ps, pageSize);

                }



                // Set the pen settings



                var ssl = psv.GetPlotStyleSheetList();

                if (ssl.Contains(styleSheet))

                {

                    psv.SetCurrentStyleSheet(ps, styleSheet);

                }



                // Copy the PlotSettings data back to the Layout



                var upgraded = false;

                if (!lay.IsWriteEnabled)

                {

                    lay.UpgradeOpen();

                    upgraded = true;

                }



                lay.CopyFrom(ps);



                if (upgraded)

                {

                    lay.DowngradeOpen();

                }

            }

        }



        /// <summary>

        /// Determine the maximum possible size for this layout.

        /// </summary>

        /// <returns>The maximum extents of the viewport on this layout.</returns>



        public static Extents2d GetMaximumExtents(this Layout lay)

        {

            // If the drawing template is imperial, we need to divide by

            // 1" in mm (25.4)



            var div = lay.PlotPaperUnits == PlotPaperUnit.Inches ? 25.4 : 1.0;



            // We need to flip the axes if the plot is rotated by 90 or 270 deg



            var doIt =

              lay.PlotRotation == PlotRotation.Degrees090 ||

              lay.PlotRotation == PlotRotation.Degrees270;



            // Get the extents in the correct units and orientation



            var min = lay.PlotPaperMargins.MinPoint.Swap(doIt) / div;

            var max =

              (lay.PlotPaperSize.Swap(doIt) -

               lay.PlotPaperMargins.MaxPoint.Swap(doIt).GetAsVector()) / div;



            return new Extents2d(min, max);

        }





        /// <summary>

        /// Sets the size of the viewport according to the provided extents.

        /// </summary>

        /// <param name="ext">The extents of the viewport on the page.</param>

        /// <param name="fac">Optional factor to provide padding.</param>



        public static void ResizeViewport(

          this Viewport vp, Extents2d ext, double fac = 1.0

        )

        {

            vp.Width = (ext.MaxPoint.X - ext.MinPoint.X) * fac;

            vp.Height = (ext.MaxPoint.Y - ext.MinPoint.Y) * fac;

            vp.CenterPoint =

              (Point2d.Origin + (ext.MaxPoint - ext.MinPoint) * 0.5).Pad();

        }



        /// <summary>

        /// Sets the view in a viewport to contain the specified model extents.

        /// </summary>

        /// <param name="ext">The extents of the content to fit the viewport.</param>

        /// <param name="fac">Optional factor to provide padding.</param>



        public static void FitContentToViewport(

          this Viewport vp, Extents3d ext, double fac = 1.0

        )

        {

            // Let's zoom to just larger than the extents



            vp.ViewCenter =

              (ext.MinPoint + ((ext.MaxPoint - ext.MinPoint) * 0.5)).Strip();



            // Get the dimensions of our view from the database extents



            var hgt = ext.MaxPoint.Y - ext.MinPoint.Y;

            var wid = ext.MaxPoint.X - ext.MinPoint.X;



            // We'll compare with the aspect ratio of the viewport itself

            // (which is derived from the page size)



            var aspect = vp.Width / vp.Height;



            // If our content is wider than the aspect ratio, make sure we

            // set the proposed height to be larger to accommodate the

            // content



            if (wid / hgt > aspect)

            {

                hgt = wid / aspect;

            }



            // Set the height so we're exactly at the extents



            vp.ViewHeight = hgt;



            // Set a custom scale to zoom out slightly (could also

            // vp.ViewHeight *= 1.1, for instance)



            vp.CustomScale *= fac;

        }

    }

}
