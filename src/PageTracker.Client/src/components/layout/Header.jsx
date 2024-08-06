import { Stack, Title, Group, UnstyledButton, AppShell, Burger } from '@mantine/core';
import HeaderRibbon from './HeaderRibbon';

function Header({ title, opened, toggle }) {

  return (
    <AppShell.Header padding="md" zIndex={201}>
      <Stack align="center" justify="center" gap="xs">
        <Title order={1} pt="lg">{title}</Title>
        <Burger mr="auto" pos="absolute" w="90%" maw="450"
          opened={opened}
          onClick={toggle}
          hiddenFrom="sm"
          size="sm"
        />
        <Group pos="absolute" w="75%" maw="900" justify="flex-end" gap="md" visibleFrom="sm">
          <UnstyledButton component="a" ml="auto" fw="600">
            <i className="fi-rs-book-bookmark" style={{ marginRight: "0.2rem" }} /> Log
          </UnstyledButton>
          <UnstyledButton component="a" fw="600"><i className="fi fi-rs-books" style={{ marginRight: "0.2rem" }} /> Books</UnstyledButton>
        </Group>
        <HeaderRibbon/>
      </Stack>
    </AppShell.Header>
  );
}

export default Header;