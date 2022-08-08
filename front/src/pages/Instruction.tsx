import React, { useEffect, useState } from "react";
import {
  Typography,
  Grid,
  Button,
  TextField,
  makeStyles,
  Link,
} from "@material-ui/core";
import { useFormik } from "formik";
import * as yup from "yup";
import { useHistory } from "react-router-dom";
import { toast } from "react-toastify";
import jwt_decode from "jwt-decode";
import VisibilityIcon from "@material-ui/icons/Visibility";

import WMaterialTable from "../components/WMaterialTable";
import AddMeerkatModal from "../components/AddMeerkatModal";

import AnimatedCard, { AnimatedCardContent } from "../components/AnimatedCard";
import api from "../utils/api";
import { getAuthToken, isAdmin } from "../utils/auth";

const useStyles = makeStyles({
  container: {
    marginTop: 100,
  },
  submitButton: {
    marginTop: 10,
    marginBottom: 10,
  },
  registerLink: {
    marginTop: 5,
  },
});

const Instruction = () => {
  const styles = useStyles();
  const history = useHistory();


    const downloadFile = () => {
        // var downloadUrl = "../logo.svg";

        // var downloading = chrome.downloads.download({
        // url : downloadUrl,
        // filename : 'my-image-again.svg',
        // conflictAction : 'uniquify'
        // });
        console.log("download");
    }

  const backToDashboard = () => {
    history.goBack();
  };

  return (
    <Grid container className={styles.container}>
      <Grid item xs={2}></Grid>
      <Grid item xs={8}>
        <AnimatedCard>
          <AnimatedCardContent>
          <Typography
              variant="body1"
            >
              1. Pobierz zipa
            </Typography>
            <a href="/Meerkat.zip" download>
            <Button variant="contained" color="primary" onClick={downloadFile} className={styles.submitButton}>
            Pobierz
            </Button>
            </a>
            <Typography
              variant="body1"
            >
              2. Rozpakuj go
            </Typography>
            <Typography
              variant="body1"
            >
              3. W pliku config.json podmień pole token na token meerkata z naszej strony
            </Typography>
            <Typography
              variant="body1"
            >
              4. Uruchom plik .exe
            </Typography>

            <Typography
              color="textSecondary"
              variant="subtitle2"
              align={"center"}
              gutterBottom
              className={styles.registerLink}
            >
              <Link onClick={backToDashboard}>Powrót</Link>
            </Typography>
          </AnimatedCardContent>
        </AnimatedCard>
      </Grid>
      <Grid item xs={2}></Grid>
    </Grid>
  );
};

export default Instruction;
