import { Text, Group } from "@mantine/core"
function Footer() {

  return (
    <Group c="dark.0" p="md">
      <i className="fi fi-rs-book-heart" />
      <Text fz={12}>Uicons by <a href="https://www.flaticon.com/uicons" title="Flaticon">Flaticon</a></Text>
    </Group>
  );
}

export default Footer;