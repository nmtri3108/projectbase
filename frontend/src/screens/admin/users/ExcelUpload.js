import React, { useState, useEffect } from "react";
import { accountService, alertService } from "../../../_services";

function ExcelUpload({ getAccounts, setOpenImportModal }) {
  const [file, setFile] = useState(null);
  const [mapping, setMapping] = useState({});
  const [fields, setFields] = useState([]);
  const [headers, setHeaders] = useState([]);
  const [columnIndex, setColumnIndex] = useState([]);
  const [uploadStep, setUploadStep] = useState(1);
  const [excelData, setExcelData] = useState([]);
  const [allSelectsFilled, setAllSelectsFilled] = useState(false);
  const [loading, setLoading] = useState(false);

  // Fetch available fields from the backend when the component mounts
  useEffect(() => {
    accountService
      .getFields()
      .then((response) => {
        setFields(response);
        alertService.success("Load Fields successfully", {
          keepAfterRouteChange: true,
        });
      })
      .catch((error) => {
        alertService.error(error);
      });
  }, []);

  // Function to update the mapping for a specific column
  const updateMapping = (column, field) => {
    setMapping({ ...mapping, [column]: field });
  };

  // Check if all select boxes have values selected
  useEffect(() => {
    const areAllSelectsFilled = fields.every((column) => !!mapping[column]);
    setAllSelectsFilled(areAllSelectsFilled);
  }, [fields, mapping]);

  const handleFileUpload = async () => {
    const formData = new FormData();
    formData.append("file", file);

    try {
      accountService
        .readExcels(formData)
        .then((response) => {
          setExcelData(response);
          setHeaders(Object.values(response[0] || {}));
          setColumnIndex(Object.keys(response[0] || {}));
          setUploadStep(2); // Move to the next step
          alertService.success("Upload Data successfully", {
            keepAfterRouteChange: true,
          });
        })
        .catch((error) => {
          alertService.error(error);
        });
    } catch (error) {
      // Handle errors
    }
  };

  const handleFinalUpload = async () => {
    setLoading(true);
    // Include the selected mapping in the request
    const formData = new FormData();
    formData.append("file", file);
    formData.append("mapping", JSON.stringify(mapping));

    try {
      accountService
        .uploadExcels(formData)
        .then((response) => {
          alertService.success("Import Data successfully", {
            keepAfterRouteChange: true,
          });
          setLoading(false);
          getAccounts();
          setOpenImportModal(false);
        })
        .catch((error) => {
          setLoading(false);
          alertService.error(error);
        });
      // Handle success or display a confirmation message
    } catch (error) {
      // Handle errors
    }
  };

  return (
    <div>
      {uploadStep === 1 && (
        <div>
          <h2>Step 1: Choose and Upload Excel File</h2>
          <input
            type="file"
            accept=".xlsx"
            onChange={(e) => setFile(e.target.files[0])}
          />
          {file && (
            <button className="btn btn-success" onClick={handleFileUpload}>
              Upload Excel
            </button>
          )}
        </div>
      )}

      {uploadStep === 2 && (
        <div>
          <h2>Step 2: Map Excel columns to fields:</h2>
          {fields.map((column) => (
            <div key={column}>
              <div className="row mt-1">
                <div className="col-4">
                  <label className="flex-1">Map column "{column}" to:</label>
                </div>
                <div className="col-8">
                  <select
                    className="form-select"
                    value={mapping[column] || ""}
                    onChange={(e) => updateMapping(column, e.target.value)}
                  >
                    <option value="">-- Select Field --</option>
                    {headers.map((field) => (
                      <option key={field} value={field}>
                        {field}
                      </option>
                    ))}
                  </select>
                </div>
              </div>
            </div>
          ))}
          {allSelectsFilled && (
            <button disabled={loading} className="btn btn-success" onClick={handleFinalUpload}>
              Upload with Mapping
            </button>
          )}
        </div>
      )}

      {uploadStep === 2 && excelData.length > 0 && (
        <div>
          <h2>Step 3: Display Excel Data</h2>
          <div class="table-responsive">
            <table class="table table-striped">
              <thead>
                <tr>
                  {headers.map((column) => (
                    <th scope="col" key={column}>
                      {column}
                    </th>
                  ))}
                </tr>
              </thead>
              <tbody>
                {excelData.length >= 2 &&
                  excelData.slice(1).map((row, rowIndex) => (
                    <tr scope="row" key={rowIndex}>
                      {columnIndex.map((column, colIndex) => (
                        <td key={colIndex}>{row[column]}</td>
                      ))}
                    </tr>
                  ))}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  );
}

export default ExcelUpload;
