//Install-Package WindowsAzure.Storage
//Update-Package –reinstall WindowsAzure.Storage
//Install-Package Microsoft.WindowsAzure.ConfigurationManager
//Update-Package –reinstall Microsoft.WindowsAzure.ConfigurationManager

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;

namespace ConsumeSASWithPolicy
{
    class Program
    {
        static void Main(string[] args)
        {
            string containerSASWithAccessPolicy = ConfigurationManager.AppSettings["SASURI"];

            if (string.IsNullOrEmpty(containerSASWithAccessPolicy))
            {
                Console.WriteLine("Please add SASURI in config file");
            }
            else
            {
                //Call the test methods with the shared access signatures created on the container, with and without the access policy.
                UseContainerSAS(containerSASWithAccessPolicy);
            }
            Console.ReadLine();
        }

        static void UseContainerSAS(string sas)
        {
            //Try performing container operations with the SAS provided.

            //Return a reference to the container using the SAS URI.
            CloudBlobContainer container = new CloudBlobContainer(new Uri(sas));

            //Create a list to store blob URIs returned by a listing operation on the container.
            List<ICloudBlob> blobList = new List<ICloudBlob>();

            //Write operation: write a new blob to the container.
            try
            {
                CloudBlockBlob blob = container.GetBlockBlobReference("blobCreatedViaSAS.txt");
                string blobContent = "This blob was created with a shared access signature granting write permissions to the container. ";
                MemoryStream msWrite = new MemoryStream(Encoding.UTF8.GetBytes(blobContent));
                msWrite.Position = 0;
                using (msWrite)
                {
                    blob.UploadFromStream(msWrite);
                }
                Console.WriteLine("Write operation succeeded for SAS " + sas);
                Console.WriteLine();
            }
            catch (StorageException e)
            {
                Console.WriteLine("Write operation failed for SAS " + sas);
                Console.WriteLine("Additional error information: " + e.Message);
                Console.WriteLine();
            }

            //List operation: List the blobs in the container.
            try
            {
                foreach (ICloudBlob blob in container.ListBlobs())
                {
                    blobList.Add(blob);
                }
                Console.WriteLine("List operation succeeded for SAS " + sas);
                Console.WriteLine();
            }
            catch (StorageException e)
            {
                Console.WriteLine("List operation failed for SAS " + sas);
                Console.WriteLine("Additional error information: " + e.Message);
                Console.WriteLine();
            }

            //Read operation: Get a reference to one of the blobs in the container and read it.
            try
            {
                //CloudBlockBlob blob = container.GetBlockBlobReference(blobList[0].Name);
                CloudBlockBlob blob = container.GetBlockBlobReference("blobCreatedViaSAS.txt");
                MemoryStream msRead = new MemoryStream();
                msRead.Position = 0;
                using (msRead)
                {
                    blob.DownloadToStream(msRead);
                    Console.WriteLine(msRead.Length);
                }
                Console.WriteLine("Read operation succeeded for SAS " + sas);
                Console.WriteLine();
            }
            catch (StorageException e)
            {
                Console.WriteLine("Read operation failed for SAS " + sas);
                Console.WriteLine("Additional error information: " + e.Message);
                Console.WriteLine();
            }
            Console.WriteLine();

            //Delete operation: Delete a blob in the container.
            //try
            //{
            //    //CloudBlockBlob blob = container.GetBlockBlobReference(blobList[0].Name);
            //    CloudBlockBlob blob = container.GetBlockBlobReference("blobCreatedViaSAS.txt");
            //    blob.Delete();
            //    Console.WriteLine("Delete operation succeeded for SAS " + sas);
            //    Console.WriteLine();
            //}
            //catch (StorageException e)
            //{
            //    Console.WriteLine("Delete operation failed for SAS " + sas);
            //    Console.WriteLine("Additional error information: " + e.Message);
            //    Console.WriteLine();
            //}
        }
    }
}
