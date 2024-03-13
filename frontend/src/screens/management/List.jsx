import React, { useState, useEffect } from "react";
import DataGrid, {
  Column,
  Grouping,
  GroupPanel,
  Pager,
  Paging,
  SearchPanel,
  Selection,
  Summary,
  TotalItem,
  HeaderFilter,
  FilterRow,
} from "devextreme-react/data-grid";
import LoadPanel from "devextreme-react/load-panel";
import Button from 'devextreme-react/button';

import { attendanceService } from "../../_services";
import { Role } from "../../_helpers/role";
import { Modal } from '../../_components';
import { Detail } from "./Detail";
import ExcelUpload from "./ExcelUpload";

const List = () => {
    const [records, setRecords] = useState([]);
    const [openModal, setOpenModal] = useState(false);
    const [openImportModal, setOpenImportModal] = useState(false);
    const [data,setData] = useState([]);

    useEffect(() => {
        getRecords();
    }, []);
  
    const getRecords = () => {
      attendanceService.getRecords().then((records) => {
        setRecords(records);
      });
    };

    const viewDetails = (data) => {
        console.log(data)
        setData(data);
        setOpenModal(true);
    }
  
    return (
      <div>
        <h1 className="text-red-400">Attendance List</h1>
        <br />
        <div className="d-flex">
            <button onClick={()=>setOpenImportModal(true)} className="btn btn-sm btn-success mb-2">Import Excel</button>
        </div>
        <DataGrid
          dataSource={records}
          showBorders={true}
          columnAutoWidth={true}
          noDataText="No users available"
          allowColumnResizing={true}
        >
          <HeaderFilter visible={true} />
          <Selection mode="single" />
          <GroupPanel visible={true} />
          <SearchPanel visible={true} highlightCaseSensitive={true} />
          <Grouping autoExpandAll={false} />
          <FilterRow visible={true} />
  
          <Column dataField="user.email" caption="Email" width="40%" />
                <Column
                    caption="Role"
                    width="30%"
                    cellRender={({ data }) =>
                        Object.keys(Role).find(roleName => Role[roleName] === data.user.role)
                    }
                />
                <Column
                    width="30%"
                    caption="Actions"
                    cellRender={({ data }) => (
                        <>
                            <Button
                                className="mr-1"
                                type="default"
                                width={90}
                                height={29} 
                                text={"Details"} 
                                onClick={() => viewDetails(data.attendanceRecords)}
                            />
                        </>
                    )}
                />
          <Pager
            allowedPageSizes={[10, 25, 50, 100]}
            showPageSizeSelector={true}
          />
          <Paging defaultPageSize={10} />
          <Summary>
            <TotalItem column="email" summaryType="count" />
          </Summary>
        </DataGrid>
        <LoadPanel
          shadingColor="rgba(0,0,0,0.4)"
          visible={records === null}
          showIndicator={true}
          shading={true}
          position={{ of: "body" }}
        />

        <Modal title={"Attendance Detail"} show={openModal} onHide={() => setOpenModal(false)} >
            <Detail data={data} />
        </Modal>

        <Modal title={"Import Excel"} show={openImportModal} onHide={() => setOpenImportModal(false)} >
            <ExcelUpload getRecords={getRecords} setOpenImportModal={setOpenImportModal}/>
        </Modal>
      </div>
    );
}

export { List }