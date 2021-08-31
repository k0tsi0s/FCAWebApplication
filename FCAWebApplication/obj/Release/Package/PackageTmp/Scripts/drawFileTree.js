function drawFileTree(e) {

	// $("#tree-container").height(); //$(document).height();
	console.log("in drawDendrongram");
	d3.select("svg").remove();

	var jsonFile = e.target.result;
	var treeData = JSON.parse(jsonFile);

	var fillColour = "mediumslateblue";
	// Calculate total nodes, max label length
	var totalNodes = 0;
	//var maxLabelLength = 20;

	var panSpeed = 200;
	var panBoundary = 20; // Within 20px from edges will pan when dragging.
	// Misc. variables
	var i = 0;
	var duration = 750;
	var root;

	// size of the diagram
	var viewerWidth = 10000;//$(document).width();
	var viewerHeight = $(document).height();
	var nodeHeight = 15;

	if (document.getElementById('attributes').checked)
		nodeHeight += 20;
	if (document.getElementById('objects').checked)
		nodeHeight += 20;
	if (document.getElementById('objectCount').checked)
		nodeHeight += 20;

	var tree = d3.layout.treelist().childIndent(20).nodeHeight(nodeHeight);
	var ul = d3.select("#tree-container").append("ul")
			.classed("treelist", true);

	d3.selectAll("li").remove();

	if (treeData.children)
		treeData.children.forEach(function(child) {
			collapse(child);
		});

	// Helper functions for collapsing and expanding nodes.
	function collapse(d) {
		if (d.children) {
			d._children = d.children;
			d._children.forEach(collapse);
			d.children = null;
		}
	}

	function expand(d) {
		if (d._children) {
			d.children = d._children;
			d.children.forEach(expand);
			d._children = null;
		}
	}

	function getAllParentalAttributes(d){
		if (d.parent) {
			var s = getAllParentalAttributes(d.parent);
			s = s + ', ' + d.attributes.toString();
			return s;
		}
		else
		{
			return d.attributes.toString();
		}
	}

	function render(treeData, parent) {
		var nodes = tree.nodes(treeData), duration = 250;

		function toggleChildren(d) {
			if (d.children) {
				d._children = d.children;
				d.children = null;
			} else if (d._children) {
				d.children = d._children;
				d._children = null;
			}
		}
		var nodeEls = ul.selectAll("li.node").data(nodes, function(d) {
			return d.id || (d.id = ++i);
		});

		var entered = nodeEls.enter().append("li").classed("node", true).style(
				"top", parent.y + "px").style("opacity", 0).style("height",
				tree.nodeHeight() + "px").on("click", function(d) {
			toggleChildren(d);
			render(treeData, d);

		}).on("mouseover", function(d) {
			if (d.children || d._children) {
				d3.select(this).classed("selected", true);
			}
		}).on("mouseout", function(d) {
			d3.selectAll(".selected").classed("selected", false);
		});
		// add arrows if it is a folder
		entered.append("span").attr(
				"class",
				function(d) {
					var icon = d.children ? " glyphicon-chevron-down"
							: d._children ? "glyphicon-chevron-right" : "";
					return "caret glyphicon " + icon;
				});

		// update caret direction
		nodeEls.select("span.caret").attr(
				"class",
				function(d) {
					var icon = d.children ? " glyphicon-chevron-down"
							: d._children ? "glyphicon-chevron-right" : "";
					return "caret glyphicon " + icon;
				});

		nodeEls
				.select("span")
				.html(
						function(d) {

							var objString = "";
							var attrString = "";

							var allAttrString ="";

							if (d._children)
								objString = d.objects.sort().toString();
							//else
							//	objString = d.own_objects.sort().toString();

							var maxStringLength = 150;

							var allParentalAttributes = getAllParentalAttributes(d);
						//	allParentalAttributes = "neds-watr lv";
							console.log("all parenta atts " + allParentalAttributes);

							if (d.attributes.toString())
								attrString = d.attributes.toString();

//							if (objString.length > maxStringLength)
//								objString = objString.substring(0,
//										maxStringLength)
//										+ '...';
//							if (attrString.length > maxStringLength)
//								attrString = attrString.substring(0,
//										maxStringLength)
										+ '...';
										
							//attrString = attrString.replace(/ /g, "&nbsp;");
							if (document.getElementById('attributes').checked) {
								attrString = "<span style=color:green title='" + allParentalAttributes + "'> <strong>Attributes:&nbsp;</strong>"
										+ attrString + "</span><br /> ";
							} else
								attrString = "";
							
							//objString = objString.replace(/ /g, "&nbsp;");
							//console.log("obje sgtting " + objString);

							if (document.getElementById('objects').checked) {
								objString = "<span style=color:blue><strong>Objects:&nbsp;</strong>"
										+ objString + "</span><br /> ";
							} else
								objString = "";

							var countString = "empty";

							if (document.getElementById('objectCount').checked) {
								countString = "<span style=color:DarkSlateBlue><strong>Object count:&nbsp;</strong>"
										+ d.ObjectCount + "</span><br /> ";
							} else
								countString = "";

							var nodeString = "";
							if (d._children || d.children) {
								nodeString = "&nbsp;Node: " + d.Node;

							} else {
								nodeString = "Node: " + d.Node;
							}
							;

							return "<strong>" + nodeString + "</strong><br/>"
									+ attrString 
									+ objString 
									+ countString;
						})

		// update position with transition
		nodeEls.transition().duration(duration).style("top", function(d) {
			return (d.y - tree.nodeHeight()) + "px";
		}).style("left", function(d) {
			return d.x + "px";
		}).style("opacity", 1);
		nodeEls.exit().remove();
	}

	render(treeData, treeData);
};