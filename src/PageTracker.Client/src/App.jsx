import '@mantine/core/styles.css';
import { MantineProvider } from '@mantine/core';
import Header from './components/layout/Header';
import Footer from './components/layout/Footer';
import PageLog from './PageLog.jsx'
import { AppShell } from '@mantine/core';

const title = "Reading Log";

function App() {
  

  return (
    <MantineProvider>
      <AppShell padding="md" withBorder={false}>

        <AppShell.Header padding="md">
          <Header title={title} />
        </AppShell.Header>

        <AppShell.Main>
          <PageLog />
        </AppShell.Main>

        <AppShell.Footer>
          <Footer />
        </AppShell.Footer>

      </AppShell>
    </MantineProvider>
  );
}

export default App
