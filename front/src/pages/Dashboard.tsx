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

const VisibilityButton = (props: any) => <VisibilityIcon {...props} />;

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

const Dashboard = () => {
  const styles = useStyles();
  const history = useHistory();

  const [meerkats, setMeerkats] = useState([]);
  const [showTokens, setShowTokens] = useState(false);
  const [isAddModalVisible, setIsAddModalVisible] = useState(false);

  const getMeerkats = async () => {
    const token = getAuthToken();
    if (!token) return;
    const decoded_jwt = jwt_decode(token) as any;
    const response = await api.get(
      isAdmin()
        ? `presenter/getAdminClients/${decoded_jwt.id}`
        : `presenter/getMeerkatsByUser/${decoded_jwt.id}`
    );
    const data = await response.json();
    console.log({ data });
    setMeerkats(data.list);
  };

  const fetchMeerkats = async () => {
    getMeerkats();
  };

  useEffect(() => {
    fetchMeerkats();
  }, []);

  const showAddMeerkatModal = () => {
    setIsAddModalVisible(true);
  };

  const hideAddMeerkatModal = (success = false) => {
    setIsAddModalVisible(false);
    if (success) fetchMeerkats();
  };

  const showMeerkatLogs = (event: any, rowData: any) => {
    console.log({ rowData });
    history.push(`/meerkatPreview/${rowData.meerkatId}`);
  };

  const redirectToInstruction = () => {
    history.push(`/instruction`);
  }

  const columns = isAdmin()
    ? [
        {
          title: "Nazwa",
          field: "name",
        },
      ]
    : [
        {
          title: "Nazwa",
          field: "name",
        },
        {
          title: "Admin",
          field: "adminInfo.email",
        },
        {
          title: "Token",
          field: "token",
          // hiddenByColumnsButton: true,
          hidden: !showTokens,
          // render: (rowData: any) => showTokens ? rowData['token'] : (new Array(rowData['token'].length)).fill("●").join("")
        },
      ];

  return (
    <Grid container className={styles.container}>
      <Grid item xs={2}></Grid>
      <Grid item xs={8}>
        <AnimatedCard>
          <AnimatedCardContent>
            <AddMeerkatModal
              open={isAddModalVisible}
              handleClose={hideAddMeerkatModal}
            />
            <WMaterialTable
              title={"Meerkats"}
              columns={columns}
              data={meerkats}
              onChangeColumnHidden={(smth: any) => {
                console.log({ smth });
              }}
              options={{
                headerStyle: { fontWeight: "bold" },
              }}
              actions={isAdmin() ? null : [
                {
                  icon: (props: any) => VisibilityButton(props),
                  tooltip: "Show tokens",
                  isFreeAction: true,
                  onClick: (event: any) => setShowTokens(!showTokens),
                },
              ]}
              add={isAdmin() ? null : showAddMeerkatModal}
              onRowClick={isAdmin() ? showMeerkatLogs : null}
            />
            <Typography
              color="textSecondary"
              variant="subtitle2"
              align={"center"}
              gutterBottom
              className={styles.registerLink}
            >
              <Link onClick={redirectToInstruction}>Zobacz w jaki sposób podpiąć meerkata aby poprawnie zbierał logi</Link>
            </Typography>
          </AnimatedCardContent>
        </AnimatedCard>
      </Grid>
      <Grid item xs={2}></Grid>
    </Grid>
  );
};

export default Dashboard;
