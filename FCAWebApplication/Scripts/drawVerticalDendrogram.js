function drawVerticalDendrogram(e) {

	// $("#tree-container").height(); //$(document).height();
	console.log("in drawDendrongram");
	d3.select("svg").remove();
	d3.selectAll("li").remove();

	var jsonFile = e.target.result;
	var treeData = JSON.parse(jsonFile);

	var fillColour = "mediumslateblue";
	// Calculate total nodes, max label length
	var totalNodes = 0;
	var maxLabelLength = 20;

	var panSpeed = 200;
	var panBoundary = 20; // Within 20px from edges will pan when dragging.
	// Misc. variables
	var i = 0;
	var duration = 750;
	var root;
	var oneLevel = true;

	// size of the diagram
	var viewerWidth = $(document).width();
	var viewerHeight = $(document).height();

	var tree = d3.layout.tree().size([ viewerHeight, viewerWidth ]);

	// define a d3 diagonal projection for use by the node paths later on.
	var diagonal = d3.svg.diagonal().projection(function(d) {
		return [ d.x, d.y ];
	});

	if (treeData.children)
		treeData.children.forEach(function(child) {
			collapse(child);
		});

	// TODO: Pan function, can be better implemented.
	function pan(domNode, direction) {
		var speed = panSpeed;
		if (panTimer) {
			clearTimeout(panTimer);
			translateCoords = d3.transform(svgGroup.attr("transform"));
			if (direction == 'left' || direction == 'right') {
				translateX = direction == 'left' ? translateCoords.translate[0]
						+ speed : translateCoords.translate[0] - speed;
				translateY = translateCoords.translate[1];
			} else if (direction == 'up' || direction == 'down') {
				translateX = translateCoords.translate[0];
				translateY = direction == 'up' ? translateCoords.translate[1]
						+ speed : translateCoords.translate[1] - speed;
			}
			scaleX = translateCoords.scale[0];
			scaleY = translateCoords.scale[1];
			scale = zoomListener.scale();
			svgGroup.transition().attr(
					"transform",
					"translate(" + translateX + "," + translateY + ")scale("
							+ scale + ")");
			d3.select(domNode).select('g.node').attr("transform",
					"translate(" + translateX + "," + translateY + ")");
			zoomListener.scale(zoomListener.scale());
			zoomListener.translate([ translateX, translateY ]);
			panTimer = setTimeout(function() {
				pan(domNode, speed, direction);
			}, 50);
		}
	}

	// Define the zoom function for the zoomable tree
	function zoom() {
		svgGroup.attr("transform", "translate(" + d3.event.translate
				+ ")scale(" + d3.event.scale + ")");
	}

	// define the zoomListener which calls the zoom function on the "zoom" event
	// constrained within the scaleExtents
	var zoomListener = d3.behavior.zoom().scaleExtent([ 0.1, 3 ]).on("zoom",
			zoom);

	// define the baseSvg, attaching a class for styling and the zoomListener
	var baseSvg = d3.select("#tree-container").append("svg").attr("width",
			viewerWidth).attr("height", viewerHeight).attr("class", "overlay")
			.call(zoomListener);

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

	var overCircle = function(d) {
		selectedNode = d;
		updateTempConnector();
	};
	var outCircle = function(d) {
		selectedNode = null;
		updateTempConnector();
	};

	// Function to center node when clicked/dropped so node doesn't get lost
	// when collapsing/moving with large amount of children.
	function centerNode(source) {
		scale = zoomListener.scale();
		x = -source.y0;
		y = -source.x0;
		x = x * scale + viewerWidth / 2;
		y = y * scale + viewerHeight / 2;
		d3.select('g').transition().duration(duration).attr("transform",
				"translate(" + x + "," + y + ")scale(" + scale + ")");
		zoomListener.scale(scale);
		zoomListener.translate([ x, y ]);
	}

	// Toggle children function
	function toggleChildren(d) {
		if (d.children) {
			d._children = d.children;
			d.children = null;
		} else if (d._children) {
			d.children = d._children;
			d._children = null;
		}
		return d;
	}

	// Toggle children on click.
	function click(d) {
		// if (d3.event.defaultPrevented)
		// return; // click suppressed
		d = toggleChildren(d);
		update(d);
		// centerNode(d);
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

	function update(source) {

		console.log("in update");
		// Compute the new height, function counts total children of root node
		// and sets tree height accordingly.
		// This prevents the layout looking squashed when new nodes are made
		// visible or looking sparse when nodes are removed
		// This makes the layout more consistent.
		var levelWidth = [ 1 ];
		var childCount = function(level, n) {

			if (n.children && n.children.length > 0) {
				if (levelWidth.length <= level + 1)
					levelWidth.push(0);

				levelWidth[level + 1] += n.children.length;
				n.children.forEach(function(d) {
					childCount(level + 1, d);
				});
			}
		};
		childCount(0, root);
		var newHeight = d3.max(levelWidth) * 80;// 25; // 25 pixels per line
		tree = tree.size([ newHeight, viewerWidth ]);

		// Compute the new tree layout.
		var nodes = tree.nodes(root).reverse(), links = tree.links(nodes);

		// Set widths between levels based on maxLabelLength.
		nodes.forEach(function(d) {
			d.y = (d.depth * (maxLabelLength * 10)); // maxLabelLength * 10px
			// alternatively to keep a fixed scale one can set a fixed depth per
			// level
			// Normalize for fixed-depth by commenting out below line
			// d.y = (d.depth * 500); //500px per level.
		});

		// Update the nodes…
		node = svgGroup.selectAll("g.node").data(nodes, function(d) {
			return d.id || (d.id = ++i);
		});

		// Enter any new nodes at the parent's previous position.
		var nodeEnter = node.enter().append("g")
		// .call(dragListener)

		// switch x0 y0 for vertical tree
		.attr("class", "node").attr("transform", function(d) {
			return "translate(" + source.x0 + "," + source.y0 + ")";
		}).on('click', click);

		nodeEnter.append("circle").attr('class', 'nodeCircle').attr("r", 0)
				.style("fill", function(d) {
					return d._children ? fillColour : "#fff";
				});

		node.selectAll("text").remove();

		node.append("text").attr("dy", ".35em").attr("text-anchor", "middle")
				.text(function(d) {
					return d.Node;
				});
	
		
		node.selectAll("title").remove();
		node
				.append("svg:title")
				.html(
						function(d) {
							var max = 800;
							//var returnString = "";
							var objString = "";
							var attrString = "";

							if (d._children) {
								objString = d.objects.toString();
							} else {
								objString = d.own_objects.toString();
							};

							if (objString.length > max)
								objString = objString.substring(0, max) + '...';
							if (attrString.length > max)
								attrString = attrString.substring(0, max)
										+ '...';

							objString = "Objects:&nbsp;" + objString
									+ "<br /> <br />";

							if (d.attributes.toString())
								attrString = "Attributes: "
									//	+ d.attributes.toString()
									+ getAllParentalAttributes(d)
										+ "<br /> <br />";

							return attrString + objString + "Object Count: " + d.ObjectCount;
						});

		// Transition nodes to their new position.
		// switch x and y for vertical
		var nodeUpdate = node.transition().duration(duration).attr("transform",
				function(d) {
					return "translate(" + d.x + "," + d.y + ")";
				});

		nodeUpdate.select("circle").attr("r", 15).style("fill", function(d) {
			return d._children ? fillColour : "#fff";
		});

		// Fade the text in
		nodeUpdate.select("text").style("fill-opacity", 1);

		// Transition exiting nodes to the parent's new position.
		var nodeExit = node.exit().transition().duration(duration).attr(
				"transform", function(d) {
					return "translate(" + source.y + "," + source.x + ")";
				}).remove();

		nodeExit.select("circle").attr("r", 0);

		nodeExit.select("text").style("fill-opacity", 0);

		// Update the links…
		var link = svgGroup.selectAll("path.link").data(links, function(d) {
			return d.target.id;
		});

		// Enter any new links at the parent's previous position.
		link.enter().insert("path", "g").attr("class", "link").attr("d",
				function(d) {
					var o = {
						x : source.x0,
						y : source.y0
					};
					return diagonal({
						source : o,
						target : o
					});
				});

		// Transition links to their new position.
		link.transition().duration(duration).attr("d", diagonal);

		// Transition exiting nodes to the parent's new position.
		link.exit().transition().duration(duration).attr("d", function(d) {
			var o = {
				x : source.x,
				y : source.y
			};
			return diagonal({
				source : o,
				target : o
			});
		}).remove();

		// Stash the old positions for transition.
		nodes.forEach(function(d) {
			d.x0 = d.x;
			d.y0 = d.y;
		});
	}

	// Append a group which holds all nodes and which the zoom Listener can act
	// upon.
	var svgGroup = baseSvg.append("g");

	// Define the root
	root = treeData;
	root.x0 = (viewerHeight) / 4;
	root.y0 = 40;

	scale = zoomListener.scale();
	x = root.y0;
	y = root.x0;

	d3.select('g').transition().duration(duration).attr("transform",
			"translate(" + x + "," + y + ")scale(" + scale + ")");
	zoomListener.scale(scale);
	zoomListener.translate([ x, y ]);

	// Layout the tree initially and center on the root node.
	update(root);
	// centerNode(root);
};