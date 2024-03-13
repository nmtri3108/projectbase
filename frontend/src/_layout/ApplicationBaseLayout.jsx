import React from 'react';
import {Nav, Alert} from '@/_components';

const ApplicationBaseLayout = ({ children }) => {
    return (
        <div className='container-fluid'>
            <Nav />
            <Alert />
            <main className='overflow-hidden'>{children}</main>
        </div>
    );
};

export default ApplicationBaseLayout;
