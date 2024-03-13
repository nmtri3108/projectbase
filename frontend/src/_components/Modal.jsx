import React from 'react';
import { Modal as BoostrapModal, Button } from 'react-bootstrap';

const Modal = (props) => {
  return (
    <BoostrapModal
      {...props}
      size="xl"
      aria-labelledby="contained-modal-title-vcenter"
      centered
    >
      <BoostrapModal.Header closeButton>
        <BoostrapModal.Title id="contained-modal-title-vcenter">
          {props.title}
        </BoostrapModal.Title>
      </BoostrapModal.Header>
      <BoostrapModal.Body>
        {props.children}
      </BoostrapModal.Body>
    </BoostrapModal>
  );
};

export { Modal }; 
