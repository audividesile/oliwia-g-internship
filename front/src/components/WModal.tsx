import React from "react";
import Modal from "@material-ui/core/Modal";
import { makeStyles } from "@material-ui/core/styles";

const modalStyle = {
    top: `50%`,
    left: `50%`,
    transform: `translate(-50%, -50%)`,
  }

const useStyles = makeStyles((theme) => ({
  paper: {
    position: "absolute",
    width: 400,
    backgroundColor: theme.palette.background.paper,
    // boxShadow: theme.shadows[5],
    padding: theme.spacing(2, 4, 3),
  },
}));

const WModal = ({ open, handleClose, handleAction, children } : any) => {
  const classes = useStyles();
  return (
    <Modal
      open={open}
      onClose={handleClose}
      aria-labelledby="simple-modal-title"
      aria-describedby="simple-modal-description"
    >
      <div style={modalStyle} className={classes.paper}>
          {children}
      </div>
    </Modal>
  );
};

export default WModal;
