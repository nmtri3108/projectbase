import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom";

import { accountService } from "../../_services";

function Details({ match }) {
  const { path } = match;
  const user = accountService.userValue;

  return (
    <>
      <div className="container" style={styles.body}>
        <div className="main-body" style={styles.mainBody}>
          <div className="row gutters-sm" style={styles.guttersSm}>
            <div className="col-md-8" style={styles.col}>
              <div className="card mb-3" style={styles.card}>
                <div className="card-body" style={styles.cardBody}>
                  <div className="row">
                    <div className="col-sm-3">
                      <h6 className="mb-0">Full Name</h6>
                    </div>
                    <div className="col-sm-9 text-secondary">
                      {user.title} {user.firstName} {user.lastName}
                    </div>
                  </div>
                  <hr />

                  <div className="row">
                    <div className="col-sm-3">
                      <h6 className="mb-0">Email</h6>
                    </div>
                    <div className="col-sm-9 text-secondary">{user.email}</div>
                  </div>
                  <hr />

                  <div className="row">
                    <div className="col-sm-3">
                      <h6 className="mb-0">Sex</h6>
                    </div>
                    <div className="col-sm-9 text-secondary">
                      {user.sex  == 0 && <>Male</>}
                      {user.sex  == 1 && <>FeMale</>}
                      {user.sex  == 2 && <>Other</>}
                    </div>
                  </div>
                  <hr />

                  <div className="row">
                    <div className="col-sm-3">
                      <h6 className="mb-0">Role</h6>
                    </div>
                    <div className="col-sm-9 text-secondary">
                      {user.role  == 0 && <>GeneralEmployee</>}
                      {user.role  == 1 && <>Administrator</>}
                      {user.role  == 2 && <>Manager</>}
                    </div>
                  </div>
                  <hr />

                  <div className="row">
                    <div className="col-sm-3">
                      <h6 className="mb-0">Type</h6>
                    </div>
                    <div className="col-sm-9 text-secondary">
                      {user.type  == 0 && <>Dev</>}
                      {user.type  == 1 && <>QA</>}
                      {user.type  == 2 && <>Manager</>}
                    </div>
                  </div>
                  <hr />

                  <div className="row">
                    <div className="col-sm-3">
                      <h6 className="mb-0">Department</h6>
                    </div>
                    <div className="col-sm-9 text-secondary">
                      {user.type  == 0 && <>Dev</>}
                      {user.type  == 1 && <>QA</>}
                      {user.type  == 2 && <>Manager</>}
                    </div>
                  </div>
                  <hr />

                  <div className="row">
                    <div className="col-sm-3">
                      <h6 className="mb-0">IsIntern</h6>
                    </div>
                    <div className="col-sm-9 text-secondary">
                      {user.isIntern ? <>True</>: <>False</>}
                    </div>
                  </div>
                  <hr />

                  <div className="row">
                    <div className="col-sm-3">
                      <h6 className="mb-0">Phone Number</h6>
                    </div>
                    <div className="col-sm-9 text-secondary">{user.phoneNumber}</div>
                  </div>
                  <hr />

                  {(user.type == 0 && user.extension)&& (
                    <>
                      <div className="row">
                        <div className="col-sm-3">
                          <h6 className="mb-0">Band</h6>
                        </div>
                        <div className="col-sm-9 text-secondary">
                          {user.extension.band  == 0 && <>A1</>}
                          {user.extension.band  == 1 && <>A2</>}
                          {user.extension.band  == 2 && <>A3</>}
                          {user.extension.band  == 3 && <>A4</>}
                          {user.extension.band  == 4 && <>A5</>}
                          {user.extension.band  == 5 && <>A6</>}
                          {user.extension.band  == 6 && <>A7</>}
                          {user.extension.band  == 7 && <>A8</>}
                        </div>
                      </div>
                      <hr />

                      <div className="row">
                        <div className="col-sm-3">
                          <h6 className="mb-0">Tech Directions</h6>
                        </div>
                        <div className="col-sm-9 text-secondary">{user.extension.techDirection}</div>
                      </div>
                      <hr />
                    </>
                  )}

                  {(user.type == 1 && user.extension)&& (
                    <>
                      <div className="row">
                        <div className="col-sm-3">
                          <h6 className="mb-0">Band</h6>
                        </div>
                        <div className="col-sm-9 text-secondary">
                          {user.extension.band  == 0 && <>A1</>}
                          {user.extension.band  == 1 && <>A2</>}
                          {user.extension.band  == 2 && <>A3</>}
                          {user.extension.band  == 4 && <>A4</>}
                          {user.extension.band  == 5 && <>A5</>}
                          {user.extension.band  == 6 && <>A6</>}
                          {user.extension.band  == 7 && <>A7</>}
                          {user.extension.band  == 8 && <>A8</>}
                        </div>
                      </div>
                      <hr />

                      <div className="row">
                        <div className="col-sm-3">
                          <h6 className="mb-0">Can Write Code</h6>
                        </div>
                        <div className="col-sm-9 text-secondary">{user.extension.CanWriteCode ? <>True</> : <>False</>}</div>
                      </div>
                      <hr />
                    </>
                  )}
        
                  {(user.type == 2 && user.extension) && (
                    <>
                      <div className="row">
                        <div className="col-sm-3">
                          <h6 className="mb-0">Manager Type</h6>
                        </div>
                        <div className="col-sm-9 text-secondary">
                          {user.extension.managerType  == 0 && <>DepartmentManager</>}
                          {user.extension.managerType  == 1 && <>GeneralManager</>}
                        </div>
                      </div>
                      <hr />
                    </>
                  )}
                </div>
              </div>
            </div>
            <div className="col-md-4" style={styles.col}>
              <div className="h-100 w-100 d-flex flex-column justify-content-center align-items-start">
                {/* Avatar Image */}
                <div className="card w-100 mt-0" style={styles.card}>
                  <div className="card-body" style={styles.cardBody}>
                    <div className="d-flex flex-column align-items-center text-center">
                      <div className="mt-3">
                        <h4>
                          {user.firstName} {user.lastName}
                        </h4>
                        <Link to={`${path}/update`} className="btn btn-primary">
                          Update
                        </Link>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </>
  );
}

const styles = {
  body: {
    marginTop: "20px",
    color: "#1a202c",
    textAlign: "left",
    backgroundColor: "#e2e8f0",
    boxShadow:
      "rgba(0, 0, 0.3) 0px 6px 8px -4px, rgba(0, 0.5, 0.4, 0.06) 0px 2px 4px -1px",
  },
  mainBody: {
    padding: "15px",
  },
  card: {
    boxShadow: "0 4px 6px -1px rgba(0,0,0,.1), 0 2px 4px -1px rgba(0,0,0,.06)",
    position: "relative",
    display: "flex",
    flexDirection: "column",
    minWidth: "0",
    wordWrap: "break-word",
    backgroundColor: "#fff",
    backgroundClip: "border-box",
    border: "0 solid rgba(0,0,0,.125)",
    borderRadius: ".25rem",
  },
  cardBody: {
    flex: "1 1 auto",
    minHeight: "1px",
    padding: "1rem",
  },
  guttersSm: {
    marginRight: "-8px",
    marginLeft: "-8px",
  },
  col: {
    paddingRight: "8px",
    paddingLeft: "8px",
  },
  mb3: {
    marginBottom: "1rem",
  },
  bgGray300: {
    backgroundColor: "#e2e8f0",
  },
  h100: {
    height: "100%",
  },
  shadowNone: {
    boxShadow: "none",
  },
};

export { Details };
