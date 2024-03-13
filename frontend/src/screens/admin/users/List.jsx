import React, { useState, useEffect } from "react";
import Swal from "sweetalert2";
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
import Button from "devextreme-react/button";
import LoadPanel from "devextreme-react/load-panel";

import { accountService } from "../../../_services";
import { Role } from "../../../_helpers/role";
import { Modal } from "../../../_components";
import { AddEdit } from "./AddEdit";
import ExcelUpload from "./ExcelUpload";

function List({ match }) {
  const [users, setUsers] = useState(null);
  const [openModal, setOpenModal] = useState(false);
  const [addMode, setAddMode] = useState(false);
  const [id, setId] = useState(0);
  const [openImportModal, setOpenImportModal] = useState(false);

  useEffect(() => {
    getUser();
  }, []);

  const getUser = () => {
    accountService.getAll().then((x) => setUsers(x));
  };

  function deleteUser(id) {
    Swal.fire({
      title: "Are you sure?",
      text: "You won't be able to revert this!",
      icon: "warning",
      showCancelButton: true,
      confirmButtonColor: "#3085d6",
      cancelButtonColor: "#d33",
      confirmButtonText: "Yes, delete it!",
    }).then((result) => {
      if (result.isConfirmed) {
        setUsers(
          users.map((x) => {
            if (x.id === id) {
              x.isDeleting = true;
            }
            return x;
          })
        );
        accountService.delete(id).then(() => {
          setUsers((users) => users.filter((x) => x.id !== id));
          Swal.fire("Deleted!", "Your record has been deleted.", "success");
        });
      }
    });
  }

  const addUser = () => {
    setAddMode(true);
    setOpenModal(true);
  };

  const updateUser = (id) => {
    setId(id);
    setAddMode(false);
    setOpenModal(true);
  };

  const onHide = () => {
    setAddMode(false);
    setOpenModal(false);
    setId(0);
    getUser();
  };

  return (
    <div>
      <h1 className="text-red-400">Users Management</h1>
      <br />
      <div className="d-flex">
        <button onClick={addUser} className="btn btn-sm btn-success mb-2 mr-2">
          Add User
        </button>
        <button
          onClick={() => setOpenImportModal(true)}
          className="btn btn-sm btn-success mb-2"
        >
          Import Excel
        </button>
      </div>
      <DataGrid
        dataSource={users}
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

        <Column dataField="title" caption="Name" width="15%" />
        <Column dataField="email" caption="Email" width="30%" />
        <Column
          caption="Role"
          width="15%"
          cellRender={({ data }) =>
            Object.keys(Role).find((roleName) => Role[roleName] === data.role)
          }
        />
        <Column
          width="40%"
          caption="Actions"
          cellRender={({ data }) => (
            <>
              {data.role !== Role.Administrator && (
                <>
                  <Button
                    className="mr-1"
                    type="default"
                    width={79}
                    height={29}
                    text={"Edit"}
                    onClick={() => updateUser(data.id)}
                  />
                  <Button
                    text={data.isDeleting ? "Deleting" : "Delete"}
                    type="danger"
                    disabled={data.isDeleting}
                    onClick={() => deleteUser(data.id)}
                    width={79}
                    height={29}
                    hint="Delete User"
                  />
                </>
              )}
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
        visible={users === null}
        showIndicator={true}
        shading={true}
        position={{ of: "body" }}
      />

      <Modal
        title={addMode ? "Add User" : "Update User"}
        show={openModal}
        onHide={() => setOpenModal(false)}
      >
        <AddEdit onHide={onHide} id={addMode ? 0 : id} />
      </Modal>

      <Modal
        title={"Import Excel"}
        show={openImportModal}
        onHide={() => setOpenImportModal(false)}
      >
        <ExcelUpload
          getAccounts={getUser}
          setOpenImportModal={setOpenImportModal}
        />
      </Modal>
    </div>
  );
}

export { List };
