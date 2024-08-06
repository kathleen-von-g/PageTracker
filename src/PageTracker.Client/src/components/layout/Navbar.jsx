import { AppShell, NavLink } from '@mantine/core';
import { Link } from 'react-router-dom';

function Navbar( toggle ) {
  return (
    <AppShell.Navbar py="md" px={20} withBorder={true}>
      <NavLink component={Link} onClick={toggle} to="/" label="Log" leftSection={<i className="fi-rs-book-bookmark" />} fw="600" />
      <NavLink component={Link} onClick={toggle} to="/books" label="Books" leftSection={<i className="fi-rs-books" />} fw="600" />
    </AppShell.Navbar>
  );
}

export default Navbar;