import { AppShell, NavLink } from '@mantine/core';
function Navbar() {
  return (
    <AppShell.Navbar py="md" px={20} withBorder={true}>
      <NavLink href="#required-for-focus" label="Log" leftSection={<i className="fi-rs-book-bookmark" />} fw="600" />
      <NavLink href="#required-for-focus" label="Books" leftSection={<i className="fi-rs-books" />} fw="600" />
    </AppShell.Navbar>
  );
}

export default Navbar;