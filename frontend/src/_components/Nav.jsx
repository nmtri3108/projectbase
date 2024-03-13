import React, { useState, useEffect } from "react";
import { NavLink, Route } from "react-router-dom";
import { accountService } from "../_services";
import images from "../_asset/images";

const styles = {
  avatar: {
    borderRadius: "50%",
    overflow: "hidden",
  },
  avatarMargin: {
    marginRight: "30px",
  },
};

function Nav() {
  const [user, setUser] = useState({});
  const [noAvatarImage, setNoAvatarImage] = useState(null);

  useEffect(() => {
    if (user) {
      images.noAvatar.then((img) => setNoAvatarImage(img));
    }

    const subscription = accountService.user.subscribe((x) => setUser(x));
    return () => subscription.unsubscribe();
  }, []);

  // only show nav when logged in
  if (!user) return null;

  return (
    <div>
      <nav className="navbar navbar-expand-md navbar-dark bg-dark">
        <div className="container-fluid">
          <NavLink exact to="/" className="navbar-brand">
            <img src="https://upload.wikimedia.org/wikipedia/en/7/76/AvePointLogo.png" width="100" height="50" />
          </NavLink>
          <div
            className="collapse navbar-collapse justify-content-between"
            id="navbarNav"
          >
            <ul className="navbar-nav">
              <div className="navbar-nav ml-8">
                <NavLink to={`/`} className="nav-item nav-link">
                  Home
                </NavLink>
                <NavLink to={`/attendance`} className="nav-item nav-link">
                  Create Attendance
                </NavLink>
                <NavLink to={`/attendance/detail`} className="nav-item nav-link">
                  View Attendance
                </NavLink>
              </div>
            </ul>
            <ul className="navbar-nav" style={styles.avatarMargin}>
              <li className="nav-item dropdown">
                <a
                  className="nav-link dropdown-toggle"
                  href="#"
                  id="avatarDropdown"
                  role="button"
                  data-bs-toggle="dropdown"
                  aria-haspopup="true"
                  aria-expanded="false"
                >
                  <img
                    src={noAvatarImage}
                    alt="Avatar"
                    className="avatar-img"
                    width="30"
                    height="30"
                  />
                  {user && user.firstName ? (
                    <span> Hi {user.firstName}!</span>
                  ) : (
                    <>Hi!</>
                  )}
                </a>
                <div
                  className="dropdown-menu dropdown-menu-end"
                  aria-labelledby="avatarDropdown"
                >
                  <NavLink to="/profile" className="dropdown-item">
                    Profile
                  </NavLink>
                  <div className="dropdown-divider" />
                  <a onClick={accountService.logout} className="dropdown-item">
                    Logout
                  </a>
                </div>
              </li>
            </ul>
          </div>
        </div>
      </nav>
      <Route path="/admin" component={AdminNav} />
    </div>
  );
}

function AdminNav({ match }) {
  const { path } = match;

  return (
    <nav className="admin-nav navbar navbar-expand navbar-light">
      <div className="navbar-nav">
        <NavLink to={`${path}/users`} className="nav-item nav-link">
          Users
        </NavLink>
      </div>
    </nav>
  );
}

export { Nav };
