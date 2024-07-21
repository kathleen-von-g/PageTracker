import '@mantine/core/styles.css';
import { MantineProvider, createTheme, Button,Group } from '@mantine/core';
import Header from './components/layout/Header';
import Footer from './components/layout/Footer';
import PageLog from './PageLog.jsx'
import { AppShell } from '@mantine/core';

const title = "Reading Log";
const theme = createTheme({
  colors: {
    'warm-red': ["#ffeaec", "#fdd4d6", "#f4a7ac", "#ec777e", "#e64f57", "#e3353f", "#e22732", "#c91a25", "#b31220", "#9e0419"],
    'teal-blue': ["#e4fdfd", "#d7f4f4", "#b3e5e5", "#8dd7d7", "#6dcbcb", "#57c3c3", "#49bfbf", "#37a8a8", "#269696", "#008282"]
  },
  scale: 1.25,
  black: "#333333"
});

function App() {
  

  return (
    <MantineProvider theme={theme} defaultColorScheme="auto">
      <AppShell padding="md" withBorder={false} header={{height: 100}}>

        <AppShell.Header padding="md">
          <Header title={title} />
        </AppShell.Header>

        <AppShell.Main>
          <PageLog />
          <Group>
            <Button color="warm-red.3"><i className="fi fi-rs-book-heart" style={{ marginRight: "0.75rem" }} /> This is a button</Button>
            <Button color="teal-blue">Teal button</Button>
          </Group>
        </AppShell.Main>

        <AppShell.Footer bg="dark.6">
          <Footer />
        </AppShell.Footer>

      </AppShell>
    </MantineProvider>
  );
}

export default App
