import React, { useEffect, useState } from "react";
import WModal from "./WModal";
import {
  Button,
  TextField,
  FormControlLabel,
  Switch,
  MenuItem,
  Select,
  FormControl,
  InputLabel,
} from "@material-ui/core";
import { makeStyles } from "@material-ui/core/styles";
// import FormControlLabel from '@material-ui/core/FormControlLabel';
// import Switch from '@material-ui/core/Switch';
import { toast } from 'react-toastify';
import { useFormik } from "formik";

import api from "../utils/api";

const useStyles = makeStyles((theme) => ({
  formControl: {
    width: "100%",
    marginTop: 10,
    marginBottom: 10,
  },
  submitButton: {
    marginTop: 20,
  },
}));

const AddMeerkatModal = (props: any) => {

  const classes = useStyles();

  const addMeerkat = async (data: any) => {
    const resp = await api.get(`clientCreator/create/${data.meerkatName}`);
    if(resp.status === 200){
        //success
        toast.success("Dodano poprawnie");
        props.handleClose(true);
    }
    else{
        toast.error("Coś poszło nie tak");
    }
  }

  const formik = useFormik({
    initialValues: {
        meerkatName: ''
    },
    onSubmit: addMeerkat,
  });

  return (
    <WModal {...props}>
      <form onSubmit={formik.handleSubmit}>
        <TextField
          fullWidth
          id="meerkatName"
          name="meerkatName"
          label="Nazwa"
          value={formik.values.meerkatName}
          onChange={formik.handleChange}
          error={
            formik.touched.meerkatName &&
            Boolean(formik.errors.meerkatName)
          }
          helperText={
            formik.touched.meerkatName && formik.errors.meerkatName
          }
          margin="normal"
        />

        <Button
          color="primary"
          variant="contained"
          fullWidth
          type="submit"
          className={classes.submitButton}
        >
          Dodaj
        </Button>
      </form>
    </WModal>
  );
};

export default AddMeerkatModal;
