import { useState } from 'react'
import { Button, TextInput, Flex, Text } from "@mantine/core";
import ErrorAlert from '../common/ErrorAlert';

function RecordFinishedOnForm({ onSubmit, error, todaysPages }) {
  const [formData, setFormData] = useState({ pageNumber: '' });

  const handleSubmit = (event) => {
    event.preventDefault();

    if (validateForm() == "") {
      onSubmit(formData);
      setFormData({ pageNumber: '' }); // Reset form
    }
  }

  const handleFormChange = (event) => {
    const { name, value } = event.target;
    setFormData(prevData => ({
      ...prevData,
      [name]: value,
    }));
  };

  const validateForm = () => {
    if (formData.pageNumber != '' && isNaN(formData.pageNumber)) {
      return "Please enter a number";
    } else {
      return "";
    }
  }

  const getHeading = (numPages) => {
    if (numPages <= 0) {
      return "Where did you get up to?";
    } else {
      return "Still reading? Where did you get up to?"
    }
  }


  return (
    <>
      {error && <ErrorAlert message={error.message} />}
      <Text ta="center" size="xl" fw={500}>{getHeading(todaysPages)}</Text>
      <form onSubmit={handleSubmit}>
        <Flex justify="center" align="flex-end" direction="row" wrap="wrap" gap="md">
          <TextInput name="pageNumber" value={formData.pageNumber} onChange={handleFormChange}
            aria-label="Page number" placeholder="Page number"
            leftSection={<i className="fi fi-rs-book-open-cover" />} error={validateForm()} />
          <Button color="teal-blue" type="submit">Submit</Button>
        </Flex>
      </form>
    </>
  );

}

export default RecordFinishedOnForm;