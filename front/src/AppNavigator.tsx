import React from "react";
import {
  BrowserRouter as Router,
  Switch,
  Route,
  Redirect,
} from "react-router-dom";

import { isAuthed } from "./utils/auth";
// Pages
import Login from "./pages/Login";
import Register from "./pages/Register";
import Dashboard from "./pages/Dashboard";
import MeerkatPreview from "./pages/MeerkatPreview";
import Instruction from "./pages/Instruction";

const AppNavigator = () => {
  return (
    <Router>
      <Switch>
        <Route path="/register">
          <Register />
        </Route>
        <Route path="/login">
          <Login />
        </Route>
        <PrivateRoute exact path="/meerkatPreview/:id" component={MeerkatPreview} />
        <PrivateRoute path="/instruction">
          <Instruction />
        </PrivateRoute>
        <PrivateRoute path="/">
          <Dashboard />
        </PrivateRoute>
      </Switch>
    </Router>
  );
};

// const authed = false;

const PrivateRoute = ({ children, ...rest }: any) => (
  <Route
    {...rest}
    render={({ location }) =>
      isAuthed() ? (
        children
      ) : (
        <Redirect
          to={{
            pathname: "/login",
            state: { from: location },
          }}
        />
      )
    }
  />
);

export default AppNavigator;
