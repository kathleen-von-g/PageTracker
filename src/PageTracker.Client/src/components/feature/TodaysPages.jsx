import { useEffect, useState } from 'react'
import TodaysPagesHeading from './TodaysPagesHeading';

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

  return (
    <>
      <TodaysPagesHeading numPagesRead={numPagesRead} error={error} />
    </>
  )
}

export default TodaysPages;
