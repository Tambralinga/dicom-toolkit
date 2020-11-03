﻿namespace SimpleDICOMToolkit.Client
{
    using Dicom;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IQueryRetrieveSCU
    {
        /// <summary>
        /// Query patients info from remote server
        /// </summary>
        /// <param name="serverIp">server IP</param>
        /// <param name="serverPort">server port</param>
        /// <param name="serverAET">server AET</param>
        /// <param name="localAET">local AET</param>
        /// <param name="patientId">patient ID</param>
        /// <param name="patientName">patient name</param>
        /// <returns>datasets</returns>
        ValueTask<List<DicomDataset>> QueryPatientsAsync(string serverIp, int serverPort, string serverAET, string localAET, string patientId = null, string patientName = null);

        /// <summary>
        /// Query studies from remote server
        /// </summary>
        /// <param name="serverIp">server IP</param>
        /// <param name="serverPort">server port</param>
        /// <param name="serverAET">server AET</param>
        /// <param name="localAET">local AET</param>
        /// <param name="patientId">patient ID</param>
        /// <param name="patientName">patient name</param>
        /// <param name="studyDateTime">study date time range</param>
        /// <returns>datasets</returns>
        ValueTask<List<DicomDataset>> QueryStudiesByPatientAsync(string serverIp, int serverPort, string serverAET, string localAET, string patientId = null, string patientName = null, DicomDateRange studyDateTime = null);

        /// <summary>
        /// query series from remote server
        /// </summary>
        /// <param name="serverIp">server IP</param>
        /// <param name="serverPort">server port</param>
        /// <param name="serverAET">server AET</param>
        /// <param name="localAET">local AET</param>
        /// <param name="studyInstanceUid">study instance UID</param>
        /// <param name="modality">modality</param>
        /// <returns>datasets</returns>
        ValueTask<List<DicomDataset>> QuerySeriesByStudyAsync(string serverIp, int serverPort, string serverAET, string localAET, string studyInstanceUid, string modality = null);

        /// <summary>
        /// query images from remote server
        /// </summary>
        /// <param name="serverIp">server IP</param>
        /// <param name="serverPort">server port</param>
        /// <param name="serverAET">server AET</param>
        /// <param name="localAET">local AET</param>
        /// <param name="studyInstanceUid">study instance UID</param>
        /// <param name="seriesInstanceUid">series instance UID</param>
        /// <param name="modality">mocalidt</param>
        /// <returns>datasets</returns>
        ValueTask<List<DicomDataset>> QueryImagesByStudyAndSeriesAsync(string serverIp, int serverPort, string serverAET, string localAET, string studyInstanceUid, string seriesInstanceUid, string modality = null);

        /// <summary>
        /// get images from remote server
        /// </summary>
        /// <param name="serverIp">server IP</param>
        /// <param name="serverPort">server port</param>
        /// <param name="serverAET">server AET</param>
        /// <param name="localAET">local AET</param>
        /// <param name="studyInstanceUid">study instance UID</param>
        /// <param name="seriesInstanceUid">series instance UID</param>
        /// <returns>images dataset</returns>
        ValueTask<List<DicomDataset>> GetImagesBySeriesAsync(string serverIp, int serverPort, string serverAET, string localAET, string studyInstanceUid, string seriesInstanceUid);

        /// <summary>
        /// get image from remote server
        /// </summary>
        /// <param name="serverIp">server IP</param>
        /// <param name="serverPort">server port</param>
        /// <param name="serverAET">server AET</param>
        /// <param name="localAET">local AET</param>
        /// <param name="studyInstanceUid">study instance UID</param>
        /// <param name="seriesInstanceUid">series instance UID</param>
        /// <param name="sopInstanceUid">SOP instance UID</param>
        /// <returns>image dataset</returns>
        ValueTask<DicomDataset> GetImagesBySOPInstanceAsync(string serverIp, int serverPort, string serverAET, string localAET, string studyInstanceUid, string seriesInstanceUid, string sopInstanceUid);

        /// <summary>
        /// send C-MOVE request
        /// </summary>
        /// <param name="serverIp">server IP</param>
        /// <param name="serverPort">server port</param>
        /// <param name="serverAET">server AET</param>
        /// <param name="localAET">local AET</param>
        /// <param name="destAET">destination AET, need registered in server</param>
        /// <param name="studyInstanceUid">study instance UID</param>
        /// <param name="seriesInstanceUid">series instance UID</param>
        /// <returns>true if success, else false</returns>
        ValueTask<bool?> MoveImagesAsync(string serverIp, int serverPort, string serverAET, string localAET, string destAET, string studyInstanceUid, string seriesInstanceUid);
    }
}
