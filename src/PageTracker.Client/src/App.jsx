import '@mantine/core/styles.css';
import './assets/styles/uicons-regular-straight.css'

import { MantineProvider, createTheme, AppShell } from '@mantine/core';
import Header from './components/layout/Header';
import Footer from './components/layout/Footer';
import PageLog from './PageLog'

const title = "Page Tracker";
const theme = createTheme({
  colors: {
    'warm-red': ["#ffeaec", "#fdd4d6", "#f4a7ac", "#ec777e", "#e64f57", "#e3353f", "#e22732", "#c91a25", "#b31220", "#9e0419"],
    'teal-blue': ["#e4fdfd", "#d7f4f4", "#b3e5e5", "#8dd7d7", "#6dcbcb", "#57c3c3", "#49bfbf", "#37a8a8", "#269696", "#008282"]
  },
  scale: 1.25,
  black: "#333333",
  fontFamily: 'Zen Kaku Gothic New, sans-serif',
  headings: {
    fontWeight: 900,
    fontFamily: 'Berkshire Swash, serif',
  }
});

function App() {
  

  return (
    <MantineProvider theme={theme} defaultColorScheme="light">
      <AppShell padding="md" withBorder={false} header={{ height: 100 }} footer={{ height:50}}>

        <AppShell.Header padding="md">
          <Header title={title} />
        </AppShell.Header>

        <AppShell.Main>
          <PageLog />
        </AppShell.Main>

        <AppShell.Footer bg="dark.6">
          <Footer />
        </AppShell.Footer>

      </AppShell>
    </MantineProvider>
  );
}

export default App
