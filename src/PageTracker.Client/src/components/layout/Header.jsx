import { Stack, Title } from '@mantine/core';
import HeaderRibbon from './HeaderRibbon';

function Header({ title }) {
  return (
    <Stack align="center" justify="center" gap="xs">
      <Title order={1} pt="lg">{title}</Title>
      <HeaderRibbon/>
    </Stack>
  );
}

export default Header;