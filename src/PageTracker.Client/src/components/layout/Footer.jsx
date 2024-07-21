import { Group } from "@mantine/core"
import Copyright from "./Copyright";
import AttributionUIcons from './AttributionUIcons';

function Footer() {

  const year = new Date().getFullYear();

  return (
    <Group c="dark.0" p="md" fz={12}>
      <i className="fi fi-rs-book-heart" />
      <Copyright year={year} />
      <AttributionUIcons />
    </Group>
  );
}

export default Footer;