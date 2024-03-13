import React, { useEffect } from "react";
import { Link } from "react-router-dom";
import { Formik, Field, Form, ErrorMessage } from "formik";
import * as Yup from "yup";

import { accountService, alertService } from "../../../_services";

function AddEdit(props) {
  const { id, onHide } = props;
  const isAddMode = id === 0;

  const initialValues = {
    title: "",
    firstName: "",
    lastName: "",
    email: "",
    role: 0,
    password: "",
    confirmPassword: "",
    type: 0,
    sex: 0,
    department: 0,
    phoneNumber: "",
    isIntern: true,
    extension: {
      band: 0,
      techDirection: "",
    },
  };

  const validationSchema = Yup.object().shape({
    title: Yup.string().required("Title is required"),
    firstName: Yup.string().required("First Name is required"),
    lastName: Yup.string().required("Last Name is required"),
    email: Yup.string().email("Email is invalid").required("Email is required"),
    role: Yup.number().required("Role is required"),
    password: Yup.string()
      .concat(isAddMode ? Yup.string().required("Password is required") : null)
      .min(6, "Password must be at least 6 characters"),
    confirmPassword: Yup.string()
      .when("password", (password, schema) => {
        if (password) return schema.required("Confirm Password is required");
      })
      .oneOf([Yup.ref("password")], "Passwords must match"),
  });

  function onSubmit(fields, { setStatus, setSubmitting }) {
    setStatus();
    fields.role = parseInt(fields.role);

    const transformFields = {
        ...fields,
        role: parseInt(fields.role),
        type: parseInt(fields.type),
        sex: parseInt(fields.sex),
        department: parseInt(fields.department),
        isIntern: Boolean(fields.isIntern),
        extension: transformExtension(fields),
    };

    if (isAddMode) {
      createUser(transformFields, setSubmitting);
    } else {
      updateUser(id, transformFields, setSubmitting);
    }
  }

  const transformExtension = (fields) => {
    if (fields.type === 0) {
      return {
        band: parseInt(fields.extension.band),
        techDirection: fields.extension.techDirection,
      };
    }

    if (fields.type === 1) {
      return {
        band: parseInt(fields.extension.band),
        canWriteCode: Boolean(fields.extension.canWriteCode),
      };
    }

    if (fields.type === 2) {
      return {
        managerType: parseInt(fields.extension.managerType),
      };
    }
  };

  function createUser(fields, setSubmitting) {
    accountService
      .create(fields)
      .then(() => {
        alertService.success("User added successfully", {
          keepAfterRouteChange: true,
        });
        onHide();
      })
      .catch((error) => {
        setSubmitting(false);
        alertService.error(error);
      });
  }

  function updateUser(id, fields, setSubmitting) {
    accountService
      .update(id, fields)
      .then(() => {
        alertService.success("Update successful", {
          keepAfterRouteChange: true,
        });
        onHide();
      })
      .catch((error) => {
        setSubmitting(false);
        alertService.error(error);
      });
  }

  return (
    <>
      <Formik
        initialValues={initialValues}
        validationSchema={validationSchema}
        onSubmit={onSubmit}
      >
        {({ errors, touched, isSubmitting, setFieldValue, values }) => {
          useEffect(() => {
            if (!isAddMode) {
              // get user and set form fields
              accountService.getById(id).then((user) => {
                const fields = [
                  "title",
                  "firstName",
                  "lastName",
                  "email",
                  "role",
                  "phoneNumber",
                  "sex",
                  "type",
                  "department",
                  "isIntern",
                  "extension"
                ];
                fields.forEach((field) => {
                  if(field === "extension"){
                    setFieldValue(field, transformExtension(user), false);
                  } else {
                    setFieldValue(field, user[field], false);
                  }
                });
              });
            }
          }, []);

          return (
            <Form>
              <div className="form-row">
                <div className="form-group col">
                  <label>Title</label>
                  <Field
                    name="title"
                    as="select"
                    className={
                      "form-control" +
                      (errors.title && touched.title ? " is-invalid" : "")
                    }
                  >
                    <option value=""></option>
                    <option value="Mr">Mr</option>
                    <option value="Mrs">Mrs</option>
                    <option value="Miss">Miss</option>
                    <option value="Ms">Ms</option>
                  </Field>
                  <ErrorMessage
                    name="title"
                    component="div"
                    className="invalid-feedback"
                  />
                </div>
                <div className="form-group col-5">
                  <label>First Name</label>
                  <Field
                    name="firstName"
                    type="text"
                    className={
                      "form-control" +
                      (errors.firstName && touched.firstName
                        ? " is-invalid"
                        : "")
                    }
                  />
                  <ErrorMessage
                    name="firstName"
                    component="div"
                    className="invalid-feedback"
                  />
                </div>
                <div className="form-group col-5">
                  <label>Last Name</label>
                  <Field
                    name="lastName"
                    type="text"
                    className={
                      "form-control" +
                      (errors.lastName && touched.lastName ? " is-invalid" : "")
                    }
                  />
                  <ErrorMessage
                    name="lastName"
                    component="div"
                    className="invalid-feedback"
                  />
                </div>
              </div>
              <div className="form-row">
                <div className="form-group col-7">
                  <label>Email</label>
                  <Field
                    name="email"
                    type="text"
                    className={
                      "form-control" +
                      (errors.email && touched.email ? " is-invalid" : "")
                    }
                  />
                  <ErrorMessage
                    name="email"
                    component="div"
                    className="invalid-feedback"
                  />
                </div>
                <div className="form-group col-5">
                  <label>Role</label>
                  <Field
                    name="role"
                    as="select"
                    className={
                      "form-control" +
                      (errors.role && touched.role ? " is-invalid" : "")
                    }
                  >
                    <option value=""></option>
                    <option value={0}>GeneralEmployee</option>
                    <option value={1}>Administrator</option>
                    <option value={2}>Manager</option>
                  </Field>
                  <ErrorMessage
                    name="role"
                    component="div"
                    className="invalid-feedback"
                  />
                </div>
              </div>
              <div className="form-row">
                <div className="form-group col">
                  <label>Sex</label>
                  <Field
                    name="sex"
                    as="select"
                    className={
                      "form-control" +
                      (errors.sex && touched.sex ? " is-invalid" : "")
                    }
                  >
                    <option value=""></option>
                    <option value={0}>Male</option>
                    <option value={1}>Female</option>
                    <option value={2}>Other</option>
                  </Field>
                  <ErrorMessage
                    name="sex"
                    component="div"
                    className="invalid-feedback"
                  />
                </div>
                <div className="form-group col-5">
                  <label>Phone Number</label>
                  <Field
                    name="phoneNumber"
                    type="text"
                    className={
                      "form-control" +
                      (errors.phoneNumber && touched.phoneNumber
                        ? " is-invalid"
                        : "")
                    }
                  />
                  <ErrorMessage
                    name="phoneNumber"
                    component="div"
                    className="invalid-feedback"
                  />
                </div>
                <div className="form-group col-5">
                  <label>Department</label>
                  <Field
                    name="department"
                    as="select"
                    className={
                      "form-control" +
                      (errors.department && touched.department
                        ? " is-invalid"
                        : "")
                    }
                  >
                    <option value=""></option>
                    <option value={0}>Dev</option>
                    <option value={1}>QA</option>
                    <option value={2}>Manager</option>
                  </Field>
                  <ErrorMessage
                    name="department"
                    component="div"
                    className="invalid-feedback"
                  />
                </div>
              </div>
              <div className="form-row">
                <div className="form-group form-check">
                  <Field
                    type="checkbox"
                    name="isIntern"
                    id="isIntern"
                    className={
                      "form-check-input " +
                      (errors.isIntern && touched.isIntern ? " is-invalid" : "")
                    }
                  />
                  <label htmlFor="isIntern" className="form-check-label">
                    Is Intern
                  </label>
                  <ErrorMessage
                    name="isIntern"
                    component="div"
                    className="invalid-feedback"
                  />
                </div>
              </div>
              <div className="form-row">
                <div className="form-group col">
                  <label>Type</label>
                  <Field
                    name="type"
                    as="select"
                    className={
                      "form-control" +
                      (errors.type && touched.type ? " is-invalid" : "")
                    }
                    onChange={(e) => {
                      const type = parseInt(e.target.value);
                      setFieldValue("type", type);

                      if (type === 0) {
                        setFieldValue("extension", {
                          band: 0,
                          techDirection: "",
                        });
                      }

                      if (type === 1) {
                        setFieldValue("extension", {
                          band: 0,
                          canWriteCode: true,
                        });
                      }

                      if (type === 2) {
                        setFieldValue("extension", {
                          managerType: "",
                        });
                      }
                    }}
                  >
                    <option value=""></option>
                    <option value={0}>Dev</option>
                    <option value={1}>QA</option>
                    <option value={2}>Manager</option>
                  </Field>
                  <ErrorMessage
                    name="type"
                    component="div"
                    className="invalid-feedback"
                  />
                </div>
              </div>

              {values.type === 0 ? (
                <>
                  <div className="form-row">
                    <div className="form-group col">
                      <label>Band</label>
                      <Field
                        name="extension.band"
                        as="select"
                        className={
                          "form-control" +
                          (errors.extension?.band && touched.extension?.band
                            ? " is-invalid"
                            : "")
                        }
                      >
                        <option value=""></option>
                        <option value={0}>A1</option>
                        <option value={1}>A2</option>
                        <option value={2}>A3</option>
                        <option value={3}>A4</option>
                        <option value={4}>A5</option>
                        <option value={5}>A6</option>
                        <option value={6}>A7</option>
                        <option value={7}>A8</option>
                      </Field>
                      <ErrorMessage
                        name="extension.band"
                        component="div"
                        className="invalid-feedback"
                      />
                    </div>
                    <div className="form-group col">
                      <label>Tech Direction</label>
                      <Field
                        name="extension.techDirection"
                        type="text"
                        className={
                          "form-control" +
                          (errors.extension?.techDirection &&
                          touched.extension?.techDirection
                            ? " is-invalid"
                            : "")
                        }
                      />
                      <ErrorMessage
                        name="extension.techDirection"
                        component="div"
                        className="invalid-feedback"
                      />
                    </div>
                  </div>
                </>
              ) : (
                <></>
              )}
              {values.type === 1 ? (
                <>
                  <div className="form-row">
                    <div className="form-group col">
                      <label>Band</label>
                      <Field
                        name="extension.band"
                        as="select"
                        className={
                          "form-control" +
                          (errors.extension?.band && touched.extension?.band
                            ? " is-invalid"
                            : "")
                        }
                      >
                        <option value=""></option>
                        <option value={0}>A1</option>
                        <option value={1}>A2</option>
                        <option value={2}>A3</option>
                        <option value={3}>A4</option>
                        <option value={4}>A5</option>
                        <option value={5}>A6</option>
                        <option value={6}>A7</option>
                        <option value={7}>A8</option>
                      </Field>
                      <ErrorMessage
                        name="extension.band"
                        component="div"
                        className="invalid-feedback"
                      />
                    </div>
                    <div className="form-group col">
                      <label>Can Write Code</label>
                      <Field
                        name="extension.canWriteCode"
                        as="select"
                        className={
                          "form-control" +
                          (errors.extension?.canWriteCode &&
                          touched.extension?.canWriteCode
                            ? " is-invalid"
                            : "")
                        }
                      >
                        <option value={true}>True</option>
                        <option value={false}>False</option>
                      </Field>
                      <ErrorMessage
                        name="extension.canWriteCode"
                        component="div"
                        className="invalid-feedback"
                      />
                    </div>
                  </div>
                </>
              ) : (
                <></>
              )}
              {values.type === 2 ? (
                <>
                  <div className="form-row">
                    <div className="form-group col">
                      <label>Manager Type</label>
                      <Field
                        name="extension.managerType"
                        as="select"
                        className={
                          "form-control" +
                          (errors.extension?.managerType &&
                          touched.extension?.managerType
                            ? " is-invalid"
                            : "")
                        }
                      >
                        <option value=""></option>
                        <option value={0}>DepartmentManager</option>
                        <option value={1}>GeneralManager</option>
                      </Field>
                      <ErrorMessage
                        name="extension.managerType"
                        component="div"
                        className="invalid-feedback"
                      />
                    </div>
                  </div>
                </>
              ) : (
                <></>
              )}
              <div className="form-row">
                <div className="form-group col">
                  <label>Password</label>
                  <Field
                    name="password"
                    type="password"
                    className={
                      "form-control" +
                      (errors.password && touched.password ? " is-invalid" : "")
                    }
                  />
                  <ErrorMessage
                    name="password"
                    component="div"
                    className="invalid-feedback"
                  />
                </div>
                <div className="form-group col">
                  <label>Confirm Password</label>
                  <Field
                    name="confirmPassword"
                    type="password"
                    className={
                      "form-control" +
                      (errors.confirmPassword && touched.confirmPassword
                        ? " is-invalid"
                        : "")
                    }
                  />
                  <ErrorMessage
                    name="confirmPassword"
                    component="div"
                    className="invalid-feedback"
                  />
                </div>
              </div>
              <div className="form-group d-flex justify-content-center">
                <button
                  type="submit"
                  disabled={isSubmitting}
                  className="btn btn-primary"
                  style={{ width: "25%" }}
                >
                  {isSubmitting && (
                    <span className="spinner-border spinner-border-sm mr-1"></span>
                  )}
                  Save
                </button>
              </div>
            </Form>
          );
        }}
      </Formik>
    </>
  );
}

export { AddEdit };
