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
      .then(response => {
        if (response.ok) {
          return response.text();
        }
        throw new Error("Could not retrieve the number of pages you've read today.")
      })
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
      .then(response => {
        if (response.ok) {
          return response.json();
        }
        throw new Error("Your reading session was not recorded.")
      })
      .then(() => fetchNumPagesRead())
      .catch(error => setError(error))
  }

  return (
    <>
      <TodaysPagesHeading numPagesRead={numPagesRead} />
      <RecordPagesForm onSubmit={handleRecordPages} error={error} />
    </>
  )
}

export default TodaysPages;
