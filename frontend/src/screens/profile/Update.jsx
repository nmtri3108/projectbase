import React, { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { Formik, Field, Form, ErrorMessage } from "formik";
import * as Yup from "yup";
import { accountService, alertService } from "../../_services";

function Update({ history }) {
  const [user, setUser] = useState(accountService.userValue);

  useEffect(() => {
    const subscription = accountService.user.subscribe((newUser) => {
      setUser(newUser);
    });

    // Cleanup the subscription when the component unmounts
    return () => subscription.unsubscribe();
  }, []);

  const initialValues = {
    title: user.title,
    firstName: user.firstName,
    lastName: user.lastName,
    password: "",
    confirmPassword: "",
    sex: user.sex,
    phoneNumber: user.phoneNumber,
  };

  const phoneRegExp =
    /^((\\+[1-9]{1,4}[ \\-]*)|(\\([0-9]{2,3}\\)[ \\-]*)|([0-9]{2,4})[ \\-]*)*?[0-9]{3,4}?[ \\-]*[0-9]{3,4}?$/;
  const validationSchema = Yup.object().shape({
    title: Yup.string().required("Title is required"),
    firstName: Yup.string().required("First Name is required"),
    lastName: Yup.string().required("Last Name is required"),
    password: Yup.string().min(6, "Password must be at least 6 characters"),
    confirmPassword: Yup.string()
      .when("password", (password, schema) => {
        if (password) return schema.required("Confirm Password is required");
      })
      .oneOf([Yup.ref("password")], "Passwords must match"),
    sex: Yup.number().required("Sex is required"),
    phoneNumber: Yup.string()
      .matches(phoneRegExp, "Invalid phone number")
      .required("Phone Number is required"),
  });

  function onSubmit(fields, { setStatus, setSubmitting }) {
    setStatus();

    accountService
      .updateSelf(user.id, fields)
      .then(() => {
        alertService.success("Update successful", {
          keepAfterRouteChange: true,
        });
        history.push(".");
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
        {({ errors, touched, isSubmitting }) => (
          <Form>
            <h1>Update Profile</h1>
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
                    (errors.firstName && touched.firstName ? " is-invalid" : "")
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
              <div className="form-group col-6">
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
              <div className="form-group col-6">
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
            </div>
            <div className="form-group"></div>
            <h3 className="pt-3">Change Password</h3>
            <p>Leave blank to keep the same password</p>
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
            <div className="form-group">
              <button
                type="submit"
                disabled={isSubmitting}
                className="btn btn-primary mr-2"
              >
                {isSubmitting && (
                  <span className="spinner-border spinner-border-sm mr-1"></span>
                )}
                Update
              </button>
              <Link to="/profile" className="btn btn-link">
                Cancel
              </Link>
            </div>
          </Form>
        )}
      </Formik>
    </>
  );
}

export { Update };
