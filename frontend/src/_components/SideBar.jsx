import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import { accountService } from "../_services";
import images from "../_asset/images";
import {
  AiOutlineUser,
  AiOutlineLogout,
  AiFillHome,
  AiFillDatabase,
  AiOutlineBars,
  AiOutlineAudit,
  AiOutlineControl 
} from "react-icons/ai";
import { Role } from "../_helpers";

const SideBar = () => {
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
    <aside className="w-100 h-100 bg-dark">
      <div className="w-100 px-3 py-4 overflow-auto rounded">
        <ul className="list-unstyled w-100">
          {/* Avatar Image */}
          <li className="w-100 h-fit d-flex flex-row flex-md-column align-items-center justify-content-center cursor-pointer">
            <div className="shrink-0 ml-2 mb-2">
              <img
                src={noAvatarImage}
                alt="Avatar"
                className="avatar-img"
                width="70"
                height="70"
              />
            </div>
            <div className="grow ml-3">
              <p className="d-none d-sm-inline-block text-sm font-weight-semibold text-white">
                {user && user.firstName ? <>Hi {user.firstName}!</> : <>Hi!</>}
              </p>
            </div>
          </li>

          {/* Home */}
          <li className="w-100">
            <Link
              to="/"
              style={{ textDecoration: "none" }}
              className={`d-flex align-items-center p-2 text-base justify-content-between  ${
                location.pathname === "/" ? "bg-light text-dark" : "text-white"
              } rounded-lg sideBarBtn`}
            >
              <div className="d-flex align-items-center">
                <AiFillHome
                  className={`${
                    location.pathname === "/" ? "text-dark" : "text-white"
                  } w-5 h-5`}
                />
                <span className="d-none d-md-inline-block ml-3">Home</span>
              </div>
            </Link>
          </li>

          {/* Profile */}
          <li className="w-100">
            <Link
              to="/profile"
              style={{ textDecoration: "none" }}
              className={`d-flex align-items-center p-2 text-base justify-content-between  ${
                location.pathname === "/profile"
                  ? "bg-light text-dark"
                  : "text-white"
              } rounded-lg sideBarBtn`}
            >
              <div className="d-flex align-items-center">
                <AiOutlineUser
                  className={`${
                    location.pathname === "/profile"
                      ? "text-dark"
                      : "text-white"
                  } w-5 h-5`}
                />
                <span className="d-none d-md-inline-block ml-3">Profile</span>
              </div>
            </Link>
          </li>

          <li className="w-100">
            <Link
              to="/attendance"
              style={{ textDecoration: "none" }}
              className={`d-flex align-items-center p-2 text-base justify-content-between  ${
                location.pathname === "/attendance"
                  ? "bg-light text-dark"
                  : "text-white"
              } rounded-lg sideBarBtn`}
            >
              <div className="d-flex align-items-center">
                <AiOutlineAudit
                  className={`${
                    location.pathname === "/attendance"
                      ? "text-dark"
                      : "text-white"
                  } w-5 h-5`}
                />
                <span className="d-none d-md-inline-block ml-3">Attendance</span>
              </div>
            </Link>
          </li>

          <li className="w-100">
            <Link
              to="/attendance/detail"
              style={{ textDecoration: "none" }}
              className={`d-flex align-items-center p-2 text-base justify-content-between  ${
                location.pathname === "/attendance/detail"
                  ? "bg-light text-dark"
                  : "text-white"
              } rounded-lg sideBarBtn`}
            >
              <div className="d-flex align-items-center">
                <AiOutlineBars
                  className={`${
                    location.pathname === "/attendance/detail"
                      ? "text-dark"
                      : "text-white"
                  } w-5 h-5`}
                />
                <span className="d-none d-md-inline-block ml-3">View My Attendance</span>
              </div>
            </Link>
          </li>

          {/* Administrator Users */}
          {user.role === Role.Administrator && (
            <li className="w-100">
              <Link
                to="/admin/users"
                style={{ textDecoration: "none" }}
                className={`d-flex align-items-center p-2 text-base justify-content-between  ${
                  location.pathname === "/admin/users"
                    ? "bg-light text-dark"
                    : "text-white"
                } rounded-lg sideBarBtn`}
              >
                <div className="d-flex align-items-center">
                  <AiFillDatabase
                    className={`${
                      location.pathname === "/admin/users"
                        ? "text-dark"
                        : "text-white"
                    } w-5 h-5`}
                  />
                  <span className="d-none d-md-inline-block ml-3">
                    Manage Users
                  </span>
                </div>
              </Link>
            </li>
          )}

          {/* Manager Users */}
          {user.role === Role.Manager && (
            <li className="w-100">
              <Link
                to="/management"
                style={{ textDecoration: "none" }}
                className={`d-flex align-items-center p-2 text-base justify-content-between  ${
                  location.pathname === "/management"
                    ? "bg-light text-dark"
                    : "text-white"
                } rounded-lg sideBarBtn`}
              >
                <div className="d-flex align-items-center">
                  <AiOutlineControl 
                    className={`${
                      location.pathname === "/management"
                        ? "text-dark"
                        : "text-white"
                    } w-5 h-5`}
                  />
                  <span className="d-none d-md-inline-block ml-3">
                    Management Attendance
                  </span>
                </div>
              </Link>
            </li>
          )}

          <li className="w-100">
            <div
              onClick={accountService.logout}
              className=" sideBarBtn d-flex align-items-center p-2 text-base font-weight-normal rounded-lg text-white hover-bg-gray-100 dark:hover-bg-gray-700"
            >
              <AiOutlineLogout className="text-white w-5 h-5" />
              <span className="d-none d-md-inline-block ml-3">Logout</span>
            </div>
          </li>
        </ul>
      </div>
    </aside>
  );
};

export { SideBar };
