import { Group, AppShell } from "@mantine/core"
import Copyright from "./Copyright";
import AttributionUIcons from './AttributionUIcons';
import DarkModeToggle from './DarkModeToggle'

function Footer() {

  const year = new Date().getFullYear();

  return (
    <AppShell.Footer bg="dark.6">
      <Group c="dark.0" p="md" fz={12} align-items="center">
        <i className="fi fi-rs-book-heart" />
        <Copyright year={year} />
        <AttributionUIcons />
        <div style={{ marginLeft: "auto" }}>
          <DarkModeToggle />
        </div>
       </Group>
    </AppShell.Footer>
  );
}

export default Footer;