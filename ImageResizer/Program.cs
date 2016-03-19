using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer
{
    class Program
    {
        static void Main(string[] args)
        {
            const String BASE_DIRECTORY = @"C:\Users\Warehouse\Pictures\DigitalFrame";

            try
            {
                DateTime startTime = DateTime.Now;
                ConvertImages(BASE_DIRECTORY);
                DateTime endTime = DateTime.Now;
                Console.WriteLine("Total convert seconds: " + endTime.Subtract(startTime).TotalSeconds.ToString());
            }
            catch(Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.ToString());
            }
            Console.WriteLine("Conversion Complete");
            Console.ReadKey();
        }

        private static void ConvertImages(String baseDirectory)
        {
            const Int32 MAX_IMAGE_WIDTH = 1024;
            
            try
            {
                String sourceDirName = "source";
                String destinationDirName = "destination";

                String sourcePath = System.IO.Path.Combine(baseDirectory, sourceDirName);
                String destinationPath = System.IO.Path.Combine(baseDirectory, destinationDirName);

                if (!System.IO.Directory.Exists(destinationPath))
                {
                    System.IO.Directory.CreateDirectory(destinationPath);
                }
                else
                {
                    DeleteOldFiles(destinationPath);
                }
                
                List<String> imagesToConvert = System.IO.Directory.GetFiles(sourcePath)
                    .Where(s => s.EndsWith(".jpg", StringComparison.InvariantCultureIgnoreCase) || s.EndsWith(".jpeg", StringComparison.InvariantCultureIgnoreCase)).ToList();
                Int32 counter = 0;


                foreach (String f in imagesToConvert)
                {
                    ConvertImage(f, destinationPath, counter, MAX_IMAGE_WIDTH);
                    counter++;
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
            }

            
        }

        private static void DeleteOldFiles (String basePath)
        {
            DateTime startDate = DateTime.Now;
            TaskFactory tf = new TaskFactory();

            foreach (String fileName in System.IO.Directory.GetFiles(basePath))
            {
                Task deleteTask = new Task(DeleteFile, fileName);
                tf.StartNew(DeleteFile, fileName).Wait();
            }
            
            DateTime endDate = DateTime.Now;
            Console.WriteLine("Total delete seconds: " + endDate.Subtract(startDate).TotalSeconds.ToString());
        }

        private static void DeleteFile(Object fileObj)
        {
            System.IO.File.Delete(fileObj.ToString());
        }

        private static void ConvertImage(String f, String destinationBasePath, Int32 counter, Int32 maxImageWidth)
        {
            String fName = System.IO.Path.GetFileName(f);
            String extension = System.IO.Path.GetExtension(fName);
            String finalPath = System.IO.Path.Combine(destinationBasePath, String.Format("{0}{1}", counter.ToString(), extension));
            System.Drawing.Image img = System.Drawing.Image.FromFile(f);
            Double aspectRatio = (double)img.Height / (double)img.Width;
            Int32 newX, newY;
            if (img.Width > maxImageWidth)
            {
                newX = maxImageWidth;
                newY = (Int32)(newX * aspectRatio);
            }
            else
            {
                newX = img.Width;
                newY = img.Height;
            }

            Console.WriteLine(String.Format("{0} {1} {2}", newX, newY, aspectRatio));

            Console.WriteLine("Beginning convert: " + fName);
            using (var resized = ImageUtilities.ResizeImage(img, newX, newY))
            {
                Console.WriteLine("Converted: " + fName);
                //save the resized image as a jpeg with a quality of 90
                ImageUtilities.SaveJpeg(finalPath, resized, 100);
                Console.WriteLine("Saved: " + finalPath);
            }
            img.Dispose();
        }
    }
}
