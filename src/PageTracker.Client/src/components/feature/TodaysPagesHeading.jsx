import { Text } from "@mantine/core";

function TodaysPagesHeading({ todaysPages }) {

  const getHeading = (numPagesRead) => {
    if (numPagesRead <= 0) {
      return '';
    }
    else if (numPagesRead == 1) {
      return "One page is still progress!";
    }
    else {
      return `Nice! You've read ${numPagesRead} pages today.`;
    }
  };

  return (
    <>{todaysPages && <Text ta="center" fw={600} size="xl"> {getHeading(todaysPages)}</Text>}</>
  );
}

export default TodaysPagesHeading;