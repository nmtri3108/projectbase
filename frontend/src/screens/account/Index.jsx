import React, { useEffect } from "react";
import { Route, Switch } from "react-router-dom";

import { accountService } from "../../_services";

import { Login } from "./Login";
import { Register } from "./Register";
import { VerifyEmail } from "./VerifyEmail";
import { ForgotPassword } from "./ForgotPassword";
import { ResetPassword } from "./ResetPassword";
import { Alert } from "../../_components";

function Account({ history, match }) {
  const { path } = match;

  useEffect(() => {
    // redirect to home if already logged in
    if (accountService.userValue) {
      history.push("/");
    }
  }, []);

  return (
    <>
      <div
      style={{
          backgroundImage: 'url(https://www.google.com/url?sa=i&url=https%3A%2F%2Fwww.dreamstime.com%2Fphotos-images%2Fbeautiful-background.html&psig=AOvVaw2pxmGULe55cVhbuxcyOg_p&ust=1707187131418000&source=images&cd=vfe&opi=89978449&ved=0CBMQjRxqFwoTCLD6kuaVk4QDFQAAAAAdAAAAABAE)',
          backgroundSize: 'cover',
          backgroundPosition: 'center',
          minHeight: '100vh',
          padding: '0', 
          margin: '0', 
        }}
      >
      <div className="container"
      >
        <Alert />
        <div className="row">
          <div className="col-sm-8 offset-sm-2 mt-5">
            <div className="card m-3" style={{opacity:'0.97'}}>
              <Switch>
                <Route path={`${path}/login`} component={Login} />
                <Route path={`${path}/register`} component={Register} />
                <Route path={`${path}/verify-email`} component={VerifyEmail} />
                <Route
                  path={`${path}/forgot-password`}
                  component={ForgotPassword}
                />
                <Route
                  path={`${path}/reset-password`}
                  component={ResetPassword}
                />
              </Switch>
            </div>
          </div>
        </div>
      </div>
      </div>
    </>
  );
}

export { Account };
