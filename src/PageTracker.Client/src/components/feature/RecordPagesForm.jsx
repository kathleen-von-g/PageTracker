import { useState } from 'react'
import { Button, TextInput, Flex, Text } from "@mantine/core";
import ErrorAlert from '../common/ErrorAlert';

// onSubmit - Function that accepts a numeric text input
function RecordPagesForm({ onSubmit, error, todaysPages }) {
  const [formData, setFormData] = useState({ numPages: '' });

  const handleSubmit = (event) => {
    event.preventDefault();

    if (validateForm() == "") {
      onSubmit(formData);
      setFormData({ numPages: '' }); // Reset form
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
    if (formData.numPages != '' && isNaN(formData.numPages)) {
      return "Please enter a number";
    } else {
      return "";
    }
  }

  const getHeading = (numPages) => {
    if (numPages <= 0) {
      return "How many pages did you just read?";
    } else {
      return "Any more pages?"
    }
  }

  return (
    <>
    {error && <ErrorAlert message={error.message} />}
    <Text ta="center" size="xl" fw={500}>{getHeading(todaysPages)}</Text>
    <form onSubmit={handleSubmit}>
      <Flex justify="center" align="flex-start" direction="row" wrap="wrap" gap="md">
        <TextInput name="numPages" value={formData.numPages} onChange={handleFormChange}
          placeholder="Number of pages" aria-label="Number of pages"
          leftSection={<i className="fi fi-rs-book-open-cover" />} error={validateForm()}  />
        <Button color="teal-blue" type="submit">Submit</Button>
      </Flex>
    </form>
    </>
  );
}

export default RecordPagesForm;
