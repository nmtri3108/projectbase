import React from 'react';
import { Route, Switch } from 'react-router-dom';

import { List } from './List';

function Management({ match }) {
    const { path } = match;
    
    return (
        <div className="p-4">
            <div className="container">
                <Switch>
                    <Route exact path={path} component={List} />
                </Switch>
            </div>
        </div>
    );
}

export { Management };