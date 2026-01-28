# Markdown Tips and tricks

## Markdown styling resources

- https://www.markdownguide.org/cheat-sheet/
- https://github.com/adam-p/markdown-here/wiki/markdown-cheatsheet
- https://jojozhuang.github.io/tutorial/mermaid-cheat-sheet/

## Code Comments
*See source code of this document for examples of code commenting in markdown*
<!-- This is a comment  -->
[comment]: # (This is also a comment)

## Mermaid

Diagram As Code

<!-- Using the ``` ``` operators -->
```mermaid
graph LR;
    A;
    A-->B;
    B---A;
```
<!-- can also use ::: ::: operators -->
:::mermaid
graph LR;    
:::

<!-- LR = Left to Right -->
<!-- RL = Right to Left -->
<!-- BT = Bottom to Top -->
<!-- TB = Top to Bottom -->

```mermaid
graph RL;
    A-->B;
```



<!-- Basic Flow chart -->
```mermaid
graph LR
    A(["Start"]) .-> B{"Decision"};
    B --"Yes" --> C(["Option <br> A"]);
    B --"No" --> D["Option <br> B"];
    B =="Error" ==> E("Error <br> Handling");
```