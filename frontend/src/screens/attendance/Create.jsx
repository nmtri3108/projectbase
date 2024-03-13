import React from 'react';
import { Formik, Field, Form, ErrorMessage } from 'formik';
import * as Yup from 'yup';

import { attendanceService, alertService } from '../../_services';

const Create = ({ history }) => {
    const initialValues = {
        type: 1,
    };

    const validationSchema = Yup.object().shape({
        type: Yup.number()
                .required('Type is required'),
    });

    function onSubmit(fields, { setStatus, setSubmitting }) {
        setStatus();
        attendanceService.createAttendance(fields.type)
            .then(() => {
                alertService.success('Create attendance successfully', { keepAfterRouteChange: true });
                history.push('/');
            })
            .catch(error => {
                setSubmitting(false);
                alertService.error(error);
            });
    }

    return (
        <Formik initialValues={initialValues} validationSchema={validationSchema} onSubmit={onSubmit}>
            {({ errors, touched, isSubmitting }) => (
                <Form>
                    <h3 className="card-header bg-dark text-white text-center">Attendance</h3>
                    <div className="card-body">
                        <div className="form-row">
                            <div className="form-group col">
                                <label>Type</label>
                                <Field name="type" as="select" className={'form-control' + (errors.type && touched.type ? ' is-invalid' : '')}>
                                    <option value=""></option>
                                    <option value={1}>Arrival</option>
                                    <option value={2}>Leave</option>
                                </Field>
                                <ErrorMessage name="type" component="div" className="invalid-feedback" />
                            </div>
                        </div>
                        <div className="form-group">
                            <button type="submit" disabled={isSubmitting} className="btn btn-primary">
                                {isSubmitting && <span className="spinner-border spinner-border-sm mr-1"></span>}
                                Submit
                            </button>
                        </div>
                    </div>
                </Form>
            )}
        </Formik>
    )
}

export { Create }