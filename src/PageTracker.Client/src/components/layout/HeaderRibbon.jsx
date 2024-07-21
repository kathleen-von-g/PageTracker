import pageTrackerLogo from '@/assets/images/book_logo_teal.svg'
import { Image, Group, Box } from '@mantine/core';

function HeaderRibbon() {
  return (
    <Group justify="center" grow>
      <Box bd="1px solid bright" w={600} />
      <Image w={40} src={pageTrackerLogo} className="logo" alt="{title} logo" />
      <Box bd="1px solid bright" w={600} />
    </Group>
  )
}

export default HeaderRibbon