import React from "react";
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
import { toast } from 'react-toastify';

import AnimatedCard, { AnimatedCardContent } from "../components/AnimatedCard";
import api from '../utils/api';
import {setAuthToken} from '../utils/auth';

interface LoginData {
  email: string;
  password: string;
}

const useStyles = makeStyles({
    container: {
        marginTop: 100,
      },
  submitButton: {
    marginTop: 20,
  },
  registerLink: {
    marginTop: 20,
  },
});

const validationSchema = yup.object({
  email: yup
    .string()
    .email("Enter a valid email")
    .required("Email is required"),
  password: yup
    .string()
    .min(8, "Password should be of minimum 8 characters length")
    .required("Password is required"),
});

const Login = () => {
  const styles = useStyles();
  const history = useHistory();

  const login = async (data: LoginData) => {
    const response = await api.post("account/login", data);
    const resp = await response.json();

    console.log({response, resp});
    if (resp.token) {
      setAuthToken(resp.token);
      history.push(`/`);
      // Go to app there...
    }
    else {
      toast.error("Email lub hasło nieprawidłowe");
    }
  };

  const formik = useFormik({
    initialValues: {
      email: "",
      password: "",
    },
    validationSchema: validationSchema,
    onSubmit: login,
  });

  const redirectToRegister = () => {
    history.push(`/register`);
  };

  return (
    <Grid container className={styles.container}>
      <Grid item xs={2}></Grid>
      <Grid item xs={8}>
        <AnimatedCard>
          <AnimatedCardContent>
            <Typography
              color="textSecondary"
              variant="h4"
              align={"center"}
              gutterBottom
            >
              Logowanie
            </Typography>

            <form onSubmit={formik.handleSubmit}>
              <TextField
                fullWidth
                id="email"
                name="email"
                label="Email"
                value={formik.values.email}
                onChange={formik.handleChange}
                error={formik.touched.email && Boolean(formik.errors.email)}
                helperText={formik.touched.email && formik.errors.email}
                margin="normal"
              />
              <TextField
                fullWidth
                id="password"
                name="password"
                label="Hasło"
                type="password"
                value={formik.values.password}
                onChange={formik.handleChange}
                error={
                  formik.touched.password && Boolean(formik.errors.password)
                }
                helperText={formik.touched.password && formik.errors.password}
                margin="normal"
              />
              <Button
                color="primary"
                variant="contained"
                fullWidth
                type="submit"
                className={styles.submitButton}
              >
                Zaloguj
              </Button>
            </form>
            <Typography
              color="textSecondary"
              variant="subtitle2"
              align={"center"}
              gutterBottom
              className={styles.registerLink}
            >
              <Link onClick={redirectToRegister}>
                Nie masz konta? Zarejestruj się tutaj
              </Link>
            </Typography>
          </AnimatedCardContent>
        </AnimatedCard>
      </Grid>
      <Grid item xs={2}></Grid>
    </Grid>
  );
};

export default Login;
