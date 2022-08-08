import React, { useEffect, useState } from "react";
import {
  Grid,
  makeStyles,
  Typography,
  Link

} from "@material-ui/core";

import { useHistory } from "react-router-dom";


import WMaterialTable from "../components/WMaterialTable";

import AnimatedCard, { AnimatedCardContent } from "../components/AnimatedCard";
import api from "../utils/api";


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

const columns = [
    {
      title: "Data",
      render: (rowData: any) => new Date(rowData.timestamp.seconds * 1000).toJSON()
    },
    {
      title: "Zużycie procesora",
      field: "cpuUsage",
    },
    {
      title: "Zużycie RAMu",
      field: "ramUsage",
    },
  ];

const MeerkatPreview = (props: any) => {
  const styles = useStyles();
  const history = useHistory();

  const [meerkatLogs, setMeerkatLogs] = useState<any>([]);

  const getMeerkatLogs = async () => {
    const { id } = props.match.params;

    const response = await api.get(
      `presenter/getLogsForMeerkat/${id}`
    );
    const data = await response.json();
    console.log({ data });
    setMeerkatLogs(data.list);
  };

  useEffect(() => {
    getMeerkatLogs();
  }, []);

  const backToDashboard = () => {
      history.goBack();
  }

  return (
    <Grid container className={styles.container}>
      <Grid item xs={2}></Grid>
      <Grid item xs={8}>
        <AnimatedCard>
          <AnimatedCardContent>
            <WMaterialTable
              title={meerkatLogs[0]?.name || ''}
              columns={columns}
              data={meerkatLogs}
              options={{
                headerStyle: { fontWeight: "bold" },
              }}
            />
            <Typography
              color="textSecondary"
              variant="subtitle2"
              align={"center"}
              gutterBottom
              className={styles.registerLink}
            >
              <Link onClick={backToDashboard}>
                Powrót
              </Link>
            </Typography>
          </AnimatedCardContent>
        </AnimatedCard>
      </Grid>
      <Grid item xs={2}></Grid>
    </Grid>
  );
};

export default MeerkatPreview;
