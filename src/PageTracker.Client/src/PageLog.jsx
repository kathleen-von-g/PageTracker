import { Stack, Center } from '@mantine/core';
import TodaysPages from './components/feature/TodaysPages';

function PageLog() {

  return (
    <Center>
      <Stack align="center" justify="center" w="95%">
        <TodaysPages />
      </Stack>
    </Center>
  )
}

export default PageLog;


