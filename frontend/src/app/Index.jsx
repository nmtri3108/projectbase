import React from 'react';
import { Route, Switch, Redirect, useLocation } from 'react-router-dom';

import { Role } from '@/_helpers';
import { PrivateRoute } from '@/_components';
import { Home } from '@/screens/home';
import { Profile } from '@/screens/profile';
import { Admin } from '@/screens/admin';
import { Account } from '@/screens/account';
import { Attendance } from '../screens/attendance/Index';
import { Management } from '../screens/management/Index';

function App() {
    const { pathname } = useLocation();  

    return (
        <>
            <Switch>
                <Redirect from="/:url*(/+)" to={pathname.slice(0, -1)} />
                <PrivateRoute exact path="/" component={Home} />
                <PrivateRoute path="/attendance" component={Attendance} />
                <PrivateRoute path="/management" roles={[Role.Manager]} component={Management} />
                <PrivateRoute path="/profile" component={Profile} />
                <PrivateRoute path="/admin" roles={[Role.Administrator]} component={Admin} />
                <Route path="/account" component={Account} />
                <Redirect from="*" to="/" />
            </Switch>
        </>
    );
}

export { App }; 