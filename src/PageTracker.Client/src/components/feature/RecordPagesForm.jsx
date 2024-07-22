import { useState } from 'react'
import { Button, TextInput, Flex } from "@mantine/core";
import ErrorAlert from '../common/ErrorAlert';

// onSubmit - Function that accepts a numeric text input
function RecordPagesForm({ onSubmit, error }) {
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

  return (
    <>
    {error && <ErrorAlert message={error.message} />}
    <form onSubmit={handleSubmit}>
      <Flex justify="center" align="flex-start" direction="row" wrap="wrap" gap="md">
        <TextInput name="numPages" value={formData.numPages} onChange={handleFormChange}
          placeholder="0" aria-label="Number of pages"
          leftSection={<i className="fi fi-rs-book-open-cover" />} error={validateForm()}  />
        <Button color="teal-blue" type="submit">Submit</Button>
      </Flex>
    </form>
    </>
  );
}

export default RecordPagesForm;
