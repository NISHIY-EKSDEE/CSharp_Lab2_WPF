using CSharp_Lab2_WPF.model;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_Lab2_WPF.service
{
    public class LocalStorageManager
    {
        private string name = "MyTopFavAnimeList";
        private FileInfo localDBFile = null;
        public bool FindLocalStorage(out List<Threat> list)
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (var drive in allDrives)
            {
                DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo($"{drive.Name}Anime");
                try
                {
                    FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles($"{name}.xlsx", SearchOption.TopDirectoryOnly);

                    foreach (var file in filesInDir)
                        if (TryParseThreats(file, out list)) 
                        {
                            localDBFile = file;
                            return true; 
                        }
                }
                catch (DirectoryNotFoundException e)
                {
                    continue;
                }
            }
            list = new List<Threat>();
            return false;
        }
        public bool TryParseThreats(FileInfo file, out List<Threat> list)
        {
            list = new List<Threat>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPkg = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet;
                try
                {
                    worksheet = excelPkg.Workbook.Worksheets.First();
                }
                catch (Exception)
                {
                    return false;
                }
                int totalRows = worksheet.Dimension.End.Row;
                if (!(worksheet.Cells[2, 1].Value.ToString() == ("Идентификатор УБИ") &&
                        worksheet.Cells[2, 2].Value.ToString() == ("Наименование УБИ") &&
                        worksheet.Cells[2, 3].Value.ToString() == ("Описание") &&
                        worksheet.Cells[2, 4].Value.ToString() == ("Источник угрозы (характеристика и потенциал нарушителя)") &&
                        worksheet.Cells[2, 5].Value.ToString() == ("Объект воздействия") &&
                        worksheet.Cells[2, 6].Value.ToString() == ("Нарушение конфиденциальности") &&
                        worksheet.Cells[2, 7].Value.ToString() == ("Нарушение целостности") &&
                        worksheet.Cells[2, 8].Value.ToString() == ("Нарушение доступности")
                    )) return false;
                for (int rowId = 3; rowId <= totalRows; rowId++)
                {
                    try
                    {
                        long id = long.Parse(worksheet.Cells[rowId, 1].Text);
                        string name = worksheet.Cells[rowId, 2].Text;
                        string description = worksheet.Cells[rowId, 3].Text;
                        string source = worksheet.Cells[rowId, 4].Text;
                        string actObject = worksheet.Cells[rowId, 5].Text;

                        string privacyViolation, integrityViolation, availabilityViolation;
                        string tmp = worksheet.Cells[rowId, 6].Text;
                        if (tmp == "1") privacyViolation = "да";
                        else if (tmp == "0") privacyViolation = "нет";
                        else throw new FormatException();
                        tmp = worksheet.Cells[rowId, 7].Text;
                        if (tmp == "1") integrityViolation = "да";
                        else if (tmp == "0") integrityViolation = "нет";
                        else throw new FormatException();
                        tmp = worksheet.Cells[rowId, 8].Text;
                        if (tmp == "1") availabilityViolation = "да";
                        else if (tmp == "0") availabilityViolation = "нет";
                        else throw new FormatException();

                        Threat threat = new Threat(id, name, description, source, actObject, privacyViolation, integrityViolation, availabilityViolation);
                        list.Add(threat);
                    }
                    catch (FormatException)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public bool DownloadFile(out FileInfo file)
        {
            file = null;
            using (var client = new WebClient())
            {
                try
                {
                    if (localDBFile != null)
                    {
                        
                        client.DownloadFile("https://bdu.fstec.ru/files/documents/thrlist.xlsx", localDBFile.FullName);
                        file = localDBFile;
                    }
                    else
                    {
                        foreach (var drive in DriveInfo.GetDrives())
                        {
                            string path = $"{drive.Name}Anime";
                            if (!Directory.Exists(path))
                            {
                                try
                                {
                                    Directory.CreateDirectory(path);
                                }
                                catch (Exception)
                                {
                                    continue;
                                }
                            }
                            
                            client.DownloadFile("https://bdu.fstec.ru/files/documents/thrlist.xlsx", $"{path}\\{name}.xlsx");
                            file = new FileInfo($"{path}\\{name}.xlsx");
                            return true;
                        }
                    }
                    return false;
                }
                catch (UnauthorizedAccessException e)
                {
                    throw e;
                }
                catch (WebException e)
                {
                    throw e;
                }
                catch (Exception)
                {

                    return false;
                }
            }
        }

        public bool SaveLocalStorage(FileInfo file)
        {
            if (localDBFile != null)
            {
                using (FileStream fsNew = File.Create(file.FullName))
                {
                    using (FileStream fsOld = File.OpenRead(localDBFile.FullName))
                    {
                        byte[] buf = new byte[1024];
                        while(fsOld.Read(buf, 0, buf.Length) > 0)
                        {
                            fsNew.Write(buf, 0, buf.Length);
                        }
                    }  
                }
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
