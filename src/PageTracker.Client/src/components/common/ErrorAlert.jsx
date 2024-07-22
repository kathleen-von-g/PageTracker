import { Alert } from '@mantine/core';

function ErrorAlert({ message }) {
  const icon = <i className="fi fi-rs-sad" />
  return (
    <Alert variant="light" color="red" title="Something went wrong" icon={icon}>
      {message}
    </Alert>
  );
}

export default ErrorAlert;