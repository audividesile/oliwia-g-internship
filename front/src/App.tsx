import React from "react";
import AppNavigator from "./AppNavigator";

import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

const App = () => (
  <>
    <AppNavigator />
    <ToastContainer />
  </>
);

export default App;
