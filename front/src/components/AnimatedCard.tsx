import React from "react";
import { CardContent, CardActions, Card } from "@material-ui/core";
import { animated, useSpring } from "react-spring";

export const AnimatedCardActions: React.FC = (props) => <>{props.children}</>;
export const AnimatedCardContent: React.FC = (props) => <>{props.children}</>;

interface AnimatedCardType {
  children:
    | React.ReactElement<
        typeof AnimatedCardContent | typeof AnimatedCardActions
      >[]
    | React.ReactElement<
        typeof AnimatedCardContent | typeof AnimatedCardActions
      >;
}

const AnimatedCard = (props: AnimatedCardType) => {
  const cardSpring = useSpring({
    from: {
      transform: "ScaleY(0)",
    },
    to: {
      transform: "ScaleY(1)",
    },
  });

  const styles = {
    card: {
      background: "rgba(255,255,255,0.90)",
      width: "100%",
    },
  };

  const drawContent = (child: any) => <CardContent>{child}</CardContent>;
  const drawActions = (child: any) => <CardActions>{child}</CardActions>;

  const mappings = React.Children.map(props.children, (child) => {
    if (child && child.type === AnimatedCardContent) {
      return drawContent(child);
    } else if (child && child.type === AnimatedCardActions) {
      return drawActions(child);
    }
  });

  return (
    <animated.div style={cardSpring}>
      <Card style={styles.card} variant="outlined">
        {mappings}
      </Card>
    </animated.div>
  );
};

export default AnimatedCard;
