import { AppShell } from '@mantine/core';
import Header from './components/layout/Header';
import Footer from './components/layout/Footer';
import PageLog from './PageLog'
import Navbar from './components/layout/Navbar';
import { useDisclosure } from '@mantine/hooks';

const title = "Page Tracker";


function App() {
  const [opened, { toggle }] = useDisclosure();

  return (
    <AppShell padding="md" withBorder={false}
      header={{ height: 100 }}
      footer={{ height: 50 }}
      navbar={{ width: 300, breakpoint: 'sm', collapsed: { desktop: true, mobile: !opened } }}>

      <Header title={title} opened={opened} toggle={toggle} />
      <Navbar />

      <AppShell.Main>
        <PageLog />
      </AppShell.Main>

      <Footer />
    </AppShell>
  );
}

export default App
