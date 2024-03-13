import React from 'react';
import { Route, Switch } from 'react-router-dom';

import { Create } from './Create';
import { Detail } from './Detail';

function Attendance({ match }) {
    const { path } = match;
    
    return (
        <div className="p-4">
            <div className="container">
                <Switch>
                    <Route exact path={path} component={Create} />
                    <Route path={`${path}/detail`} component={Detail} />
                </Switch>
            </div>
        </div>
    );
}

export { Attendance };