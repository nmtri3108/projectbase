import React from 'react';
import { Route, Redirect } from 'react-router-dom';
import { Role } from '@/_helpers';
import { accountService } from '../_services';
import DashBoardLayout from '../_layout/DashBoardLayout';
import ApplicationBaseLayout from '../_layout/ApplicationBaseLayout';

function PrivateRoute({ component: Component, roles, ...rest }) {
    return (
        <Route {...rest} render={props => {
            const user = accountService.userValue;
            if (!user) {
                // not logged in so redirect to login page with the return url
                return <Redirect to={{ pathname: '/account/login', state: { from: props.location } }} />
            }

            // check if route is restricted by role
            if (roles && roles.indexOf(user.role) === -1) {
                // role not authorized so redirect to home page
                return <Redirect to={{ pathname: '/'}} />
            }

            // authorized so return component
            return user.role === Role.Administrator || user.role === Role.Manager ? (
                <DashBoardLayout>
                    <Component {...props} />
                </DashBoardLayout>
            ) :(
                <ApplicationBaseLayout>
                    <Component {...props} />
                </ApplicationBaseLayout>
            )  
        }} />
    );
}

export { PrivateRoute };