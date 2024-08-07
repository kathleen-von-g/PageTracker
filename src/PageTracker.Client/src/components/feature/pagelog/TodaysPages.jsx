import { useEffect, useState } from 'react'
import { Tabs, Stack, Text } from "@mantine/core";
import TodaysPagesHeading from './TodaysPagesHeading';
import RecordPagesForm from './RecordPagesForm';
import RecordFinishedOnForm from './RecordFinishedOnForm';

const API_URL = "/api/reading-session";

function TodaysPages() {
  const [numPagesRead, setNumPagesRead] = useState(0);
  const [error, setError] = useState(null)

  // Start by getting the number of pages read today
  useEffect(() => {
    fetchNumPagesRead();
  }, []);


  const fetchNumPagesRead = () => {
    setError('');
    fetch(`${API_URL}/pages`)
      .then(response => {
        if (response.ok) {
          return response.text();
        }
        throw new Error("Could not retrieve the number of pages you've read today.")
      })
      .then(text => setNumPagesRead(text))
      .catch(error => setError(error))
  }

  const handleRecordPages = (data) => {
    setError('');
    console.log(`Recording ${data.numPages} page/s read`);
    const parsed = Number.parseInt(data.numPages);
    fetch(`${API_URL}/pages/${parsed}`, { method: 'POST' })
      .then(response => {
        if (response.ok) {
          return response.json();
        }
        throw new Error("Your reading session was not recorded.")
      })
      .then(() => fetchNumPagesRead())
      .catch(error => setError(error))
  }

  const handleRecordFinishedOn = (data) => {
    setError('');
    console.log(`Recording that the page finished on was ${data.pageNumber}`);
    const parsed = Number.parseInt(data.pageNumber);
    fetch(`${API_URL}/finished-at/${parsed}`, { method: 'POST' })
      .then(response => {
        if (response.ok) {
          return response.json();
        }
        throw new Error("Your reading session was not recorded.")
      })
    .then(() => fetchNumPagesRead())
    .catch(error => setError(error));
  }

  return (
    <>
      <Tabs w="100%" maw="700" color="warm-red.3" defaultValue="finished-on" variant="outline" pt="20" inverted>
        <Tabs.Panel value="finished-on">
          <Stack mih="100" align="center" justify="center" gap="xs">
            <TodaysPagesHeading todaysPages={numPagesRead} />
            <RecordFinishedOnForm onSubmit={handleRecordFinishedOn} error={error} todaysPages={numPagesRead} />
          </Stack>
        </Tabs.Panel>
        <Tabs.Panel value="pages">
          <Stack mih="100" align="center" justify="center" gap="xs">
            <TodaysPagesHeading todaysPages={numPagesRead} />
            <RecordPagesForm onSubmit={handleRecordPages} error={error} todaysPages={numPagesRead} />
          </Stack>
        </Tabs.Panel>

        <Tabs.List mt="50" justify="flex-end" >
          <Text pt="6" mr="auto">Log by</Text>
          <Tabs.Tab value="finished-on">
            Finished on
          </Tabs.Tab>
          <Tabs.Tab value="pages">
            Pages
          </Tabs.Tab>
        </Tabs.List>
      </Tabs>  
    </>
  )
}

export default TodaysPages;
