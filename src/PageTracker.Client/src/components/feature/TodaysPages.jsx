import { useEffect, useState } from 'react'
import TodaysPagesHeading from './TodaysPagesHeading';
import RecordPagesForm from './RecordPagesForm';

const API_URL = "/api/reading-session/pages";

function TodaysPages() {
  const [numPagesRead, setNumPagesRead] = useState(0);
  const [error, setError] = useState(null)

  // Start by getting the number of pages read today
  useEffect(() => {
    fetchNumPagesRead();
  }, []);

  // Mock the API call
  const fetchNumPagesRead = () => {
    fetch(API_URL)
      .then(response => response.text())
      .then(text => setNumPagesRead(text))
      .catch(error => setError(error))
  }

  const handleRecordPages = (data) => {
    console.log(`Recording ${data.numPages} page/s read`);
    const parsed = Number.parseInt(data.numPages);
    fetch(`${API_URL}/${parsed}`,
      {
        method: 'POST'
      })
      .then(() => fetchNumPagesRead())
      .catch(error => setError(error))
  }

  return (
    <>
      <TodaysPagesHeading numPagesRead={numPagesRead} error={error} />
      <RecordPagesForm onSubmit={handleRecordPages} />
    </>
  )
}

export default TodaysPages;
