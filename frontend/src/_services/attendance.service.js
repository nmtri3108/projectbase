import { fetchWrapper } from "../_helpers";
import config from "config";

const apiUrl = config.apiUrl;
const baseUrl = `${apiUrl}/Attendance`;

export const attendanceService = {
  getRecordsForEachEmployee,
  createAttendance,
  getRecords,
  getFields,
  readExcels,
  uploadExcels
};

function getRecordsForEachEmployee() {
  return fetchWrapper.get(baseUrl);
}

function getRecords() {
    return fetchWrapper.get(`${baseUrl}/managed`);
  }
  

function createAttendance(type) {
  return fetchWrapper.post(`${baseUrl}/${type}`);
}

//excel
function getFields() {
    return fetchWrapper.get(`${baseUrl}/object-fields`);
  }
  
function readExcels(fromData) {
return fetchWrapper.postFormData(`${baseUrl}/read-excel`, fromData);
}

function uploadExcels(fromData) {
return fetchWrapper.postFormData(`${baseUrl}/import`, fromData);
}
